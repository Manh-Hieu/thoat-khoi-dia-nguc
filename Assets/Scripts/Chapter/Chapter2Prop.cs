using UnityEngine;
using EscapeFromHell.Systems;

namespace EscapeFromHell.Chapter
{
    public enum Chapter2PropType
    {
        TaiXiuTable,
        Recruiter,
        PokerTable,
        RouletteTable,
        LeftDoorGuard1,
        LeftDoorGuard2,
        VipStairsGuard,
        LeftExitDoor
    }

    public class Chapter2Prop : InteractableObject
    {
        [Header("Prop Configuration")]
        [SerializeField] private Chapter2PropType propType;

        public void Initialize(Chapter2PropType type, string prompt)
        {
            propType = type;
            PromptMessage = prompt;
        }

        public override void Interact()
        {
            if (Chapter2Controller.Instance == null)
            {
                Debug.LogWarning("Chapter2Controller Instance not found!");
                return;
            }

            switch (propType)
            {
                case Chapter2PropType.TaiXiuTable:
                    Chapter2Controller.Instance.OpenTaiXiuUI();
                    break;
                case Chapter2PropType.Recruiter:
                    Chapter2Controller.Instance.InteractWithRecruiter();
                    break;
                case Chapter2PropType.PokerTable:
                    Chapter2Controller.Instance.InteractWithPokerTable();
                    break;
                case Chapter2PropType.RouletteTable:
                    Chapter2Controller.Instance.InteractWithRouletteTable();
                    break;
                case Chapter2PropType.LeftDoorGuard1:
                    Chapter2Controller.Instance.InteractWithLeftDoorGuard1();
                    break;
                case Chapter2PropType.LeftDoorGuard2:
                    Chapter2Controller.Instance.InteractWithLeftDoorGuard2();
                    break;
                case Chapter2PropType.VipStairsGuard:
                    Chapter2Controller.Instance.InteractWithVipStairsGuard();
                    break;
                case Chapter2PropType.LeftExitDoor:
                    Chapter2Controller.Instance.InteractWithLeftExitDoor();
                    break;
            }
        }
    }
}
