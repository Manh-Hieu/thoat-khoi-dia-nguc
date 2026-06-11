using UnityEngine;
using EscapeFromHell.Systems;

namespace EscapeFromHell.Chapter
{
    public enum Chapter1PropType
    {
        Phone,
        Laptop,
        Bills
    }

    public class Chapter1Prop : InteractableObject
    {
        [Header("Prop Configuration")]
        [SerializeField] private Chapter1PropType propType;

        public override void Interact()
        {
            if (Chapter1Controller.Instance == null)
            {
                Debug.LogWarning("Chapter1Controller Instance not found!");
                return;
            }

            switch (propType)
            {
                case Chapter1PropType.Phone:
                    Chapter1Controller.Instance.ReadPhone();
                    break;
                case Chapter1PropType.Laptop:
                    Chapter1Controller.Instance.ReadLaptop();
                    break;
                case Chapter1PropType.Bills:
                    Chapter1Controller.Instance.ReadBills();
                    break;
            }
        }
    }
}
