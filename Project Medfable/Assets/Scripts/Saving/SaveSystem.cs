using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Medfable.Saving
{
    public class SaveSystem : MonoBehaviour
    {
        private const string prevSceneIndex = "Previous Scene";

        // Merges previous game state with the current one and then saves the file
        public void Save(string saveFile)
        {
            Dictionary<string, object> gameState = LoadFile(saveFile);
            CatchGameState(gameState);
            SaveFile(saveFile, gameState);
        }

        // Saves current file via serialization of the game's state
        private void SaveFile(string saveFile, Dictionary<string, object> gameState)
        {
            string path = GetFilePath(saveFile);
            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, gameState);
            }
        }

        // Allows the player to load up their most current file for the scene
        public void Load(string loadFile)
        {
            GetGameState(LoadFile(loadFile));
        }

        /*Loads the last scene the player saved in via searching the game state dictionary for 
        * corresponding index
        */
        public IEnumerator LoadLastScene(string loadFile)
        {
            Dictionary<string, object> gameState = LoadFile(loadFile);
            if (!gameState.ContainsKey(prevSceneIndex))
            {
                GetGameState(gameState);
                yield break; 
            }

            int prevIndex = (int)gameState[prevSceneIndex];
            if (prevIndex != SceneManager.GetActiveScene().buildIndex)
            {
                yield return SceneManager.LoadSceneAsync(prevIndex);
            }

            GetGameState(gameState);
        }

        // Deletes the last save file if it exists
        public void DeleteSave(string saveFile)
        {
            string path = GetFilePath(saveFile);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /*Loads up most current save file by deserializing the file otherwise if a
        * load file doesn't exist it will create a new dict for whenever the user saves
        */
        private Dictionary<string, object> LoadFile(string loadFile)
        {
            string path = GetFilePath(loadFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(fileStream);
            }
        }

        // Gets the file path of the save file
        private string GetFilePath(string loadFile)
        {
            return Path.Combine(Application.persistentDataPath, loadFile);
        }

        // Restores a prior game state by checking all the savable entities in a saved dictionary
        private void GetGameState(Dictionary<string, object> gameState)
        {
            if (gameState.Count == 0) { return; }
            
            foreach (EntitySavable entity in FindObjectsOfType<EntitySavable>())
            {
                string entityGuid = entity.GetGuid();
                if (gameState.ContainsKey(entityGuid))
                {
                    entity.GetObjState(gameState[entityGuid]);
                }
            }
        }

        /*Capture the current game state by getting all the savable entites and adding it to
        * a dictionary
        */
        private void CatchGameState(Dictionary<string, object> gameState)
        {
            foreach (EntitySavable entity in FindObjectsOfType<EntitySavable>())
            {
                string entityGuid = entity.GetGuid();
                gameState[entityGuid] = entity.CatchObjState();
            }
            gameState[prevSceneIndex] = SceneManager.GetActiveScene().buildIndex;
        }
    }
}
