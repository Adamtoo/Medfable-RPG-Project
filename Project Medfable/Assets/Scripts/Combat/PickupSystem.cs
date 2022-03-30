using UnityEngine;

namespace Medfable.Combat
{
    public class PickupSystem : MonoBehaviour
    {
        [SerializeField]
        WeaponSystem weapon = null;
        private bool isPickedUp = false;

        // Allows the player to pickup a weapon to equip
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !isPickedUp)
            {
                other.GetComponent<EntityCombat>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
