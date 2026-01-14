using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text tutorialText;

    public PlayerMovement player;
    public WeaponHolder weaponHolder;
    public GrenadeThrow grenadeThrow;

    public Target[] primaryTargets;
    public Target[] secondaryTargets;
    public Target[] grenadeTargets;

    public WeaponPickup primaryPickup;
    public WeaponPickup secondaryPickup;
    public GrenadePickup grenadePickup;
    public AmmoPickup ammoPickup;

    int step;

    void Start()
    {
        Show("Use the joystick to move forward");
    }

    void Update()
    {
        switch (step)
        {
            case 0:
                if (player.HasMoved())
                    Next("Hold RUN to sprint");
                break;

            case 1:
                if (player.IsSprinting())
                    Next("Tap JUMP to jump");
                break;

            case 2:
                if (player.HasJumped())
                    Next("Tap CROUCH to crouch");
                break;

            case 3:
                if (player.IsCrouching())
                    Next("Go PRONE to crawl");
                break;

            case 4:
                if (player.IsProne())
                    Next("Climb the ladder");
                break;

            case 5:
                if (player.HasUsedLadder())
                    Next("Climb the wall");
                break;

            case 6:
                if (player.HasClimbedWall())
                {
                    primaryPickup.gameObject.SetActive(true);
                    Next("Pick up the Primary Weapon");
                }
                break;

            case 7:
                if (weaponHolder.HasPrimary())
                {
                    EnableTargets(primaryTargets);
                    Next("Shoot all targets");
                }
                break;

            case 8:
                if (AllDown(primaryTargets))
                {
                    secondaryPickup.gameObject.SetActive(true);
                    Next("Pick up the Secondary Weapon");
                }
                break;

            case 9:
                if (weaponHolder.HasSecondary())
                {
                    EnableTargets(secondaryTargets);
                    Next("Switch weapon and shoot all targets");
                }
                break;

            case 10:
                if (AllDown(secondaryTargets))
                {
                    ammoPickup.gameObject.SetActive(true);
                    Next("Use Ammo Box to refill BBs");
                }
                break;

            case 11:
                if (!ammoPickup.gameObject.activeSelf)
                {
                    grenadePickup.gameObject.SetActive(true);
                    Next("Pick up grenades");
                }
                break;

            case 12:
                if (grenadeThrow.grenadeCount >= 3)
                {
                    EnableTargets(grenadeTargets);
                    Next("Throw grenades at the targets");
                }
                break;

            case 13:
                if (AllDown(grenadeTargets))
                    Show("CONGRATULATIONS!\nYou completed the tutorial!");
                break;
        }
    }

    void Show(string msg)
    {
        tutorialText.text = msg;
    }

    void Next(string msg)
    {
        step++;
        Show(msg);
    }

    void EnableTargets(Target[] t)
    {
        foreach (var x in t)
            x.gameObject.SetActive(true);
    }

    bool AllDown(Target[] t)
    {
        foreach (var x in t)
            if (x.gameObject.activeSelf)
                return false;
        return true;
    }
}
