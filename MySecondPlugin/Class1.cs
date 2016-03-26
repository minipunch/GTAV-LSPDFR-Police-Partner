// Required attribute to provide the hook with information about this plugin.
[assembly: Rage.Attributes.Plugin("MINIPUNCH PLUGIN", Author = "MINIPUNCH", Description = "BY MINIPUNCH")]

namespace bodyguardPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Rage;
    using Rage.Native;

    internal static class EntryPoint
    {
        // our bodyguard group
        public static Group bodyguardGroup = null;
        // our bodyguard
        public static Ped bodyguard = null;
        // the player
        public static Ped playerPed = null;
        // partner count
        private static int partnerCount = 0;
        // has spawned once already flag
        private static bool hasSpawnedAlready = false;

        /// <summary>
        /// Defines the entry point of the plugin.
        /// </summary>
        private static void Main()
        {
            while (true)
            {
                // Yield to other plugins
                GameFiber.Yield();
                // Key to spawn bodyguard (PageUp) - make sure only one exists
                if (Game.IsKeyDown(Keys.PageUp) && partnerCount < 1)
                {
                    // assign playerPed character to user's character
                    playerPed = Game.LocalPlayer.Character;
                    // Create bodyguard group
                    bodyguardGroup = playerPed.Group;
					// set spawn locatation for bodyguard
                    Vector3 spawnLocation = playerPed.GetOffsetPosition(Vector3.RelativeFront * 10f);
                    // spawn new bodyguard
					bodyguard = new Ped("S_M_Y_Sheriff_01", spawnLocation, 0f);
                    bodyguard.BlockPermanentEvents = true;
                    // give him a pistol
                    NativeFunction.Natives.GiveWeaponToPed(bodyguard,0x1B06D571,450, true, true);
					// make him a cop
                    NativeFunction.Natives.SetPedAsCop(bodyguard, true);
					// display notification
                    Game.DisplayNotification("Your partner is ready for patrol!");
                    // increment partner count
                    partnerCount++;
                    // set spawned once already flag
                    hasSpawnedAlready = true;
                }
                else if(Game.IsKeyDown(Keys.PageUp) && partnerCount >= 1)
                {
                    Game.DisplayNotification("You already have a partner. Cannot have more than one at a time.");
                }

				// if bodyguard exists, but for some reason is not in the player's group
                if (bodyguard != null && (bodyguardGroup.Count - 1) < 1)
                {
                    // make sure he is always a member of the player group
                    bodyguardGroup.AddMember(bodyguard);
                }

                // If the bodyguard has been created, and your player is in a police car, but the bodyguard is not
                if (bodyguard != null && playerPed.IsInAnyPoliceVehicle && !(bodyguard.IsInAnyPoliceVehicle))
                {

                    // set the proper car target for entry
                    Vehicle leaderVehicle = playerPed.CurrentVehicle;

                    // move the bodyguard to the targetted car
                    bodyguard.Tasks.GoToOffsetFromEntity(leaderVehicle, 0, 5.0f, 3.0f, 2.0f).WaitForCompletion();

                    // enter the vehicle
                    bodyguard.Tasks.EnterVehicle(leaderVehicle, 0).WaitForCompletion();

                }

                // If the bodyguard is in a vehicle, while you are not
                if(bodyguard != null && bodyguard.IsInAnyPoliceVehicle && playerPed.DistanceTo2D(bodyguard.GetOffsetPosition(Vector3.RelativeFront)) > 5f)
                {
                    // make the bodyguard get out and follow you
                    bodyguard.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                    bodyguard.Tasks.GoToOffsetFromEntity(playerPed, 0, 5.0f, 3.0f, 2.0f).WaitForCompletion();
                }

                // Reset partner count if partner dies or doesn't exist anymore
                if(hasSpawnedAlready && (!(bodyguard.IsAlive) || !(bodyguard.Exists())))
                {
                    partnerCount = 0;
                    bodyguard = null;
                }

                // PageDown to make partner "stand down"
                if(bodyguard != null && Game.IsKeyDown(Keys.PageDown))
                {
                    if(bodyguard.IsShooting || bodyguard.IsWeaponReadyToShoot)
                    {
                        bodyguard.Tasks.ClearImmediately();
                        Game.DisplayNotification("Partner's tasks have been cleared! He should now be following you!");
                    }
                    else
                    {
                        Game.DisplayNotification("Your partner is not shooting or aiming at anything. He should be following you properly.");
                    }
                }
                else if(bodyguard == null && Game.IsKeyDown(Keys.PageDown))
                {
                    Game.DisplayNotification("You must spawn a partner first before making him/her stand down!");
                }
   
            }

        }
    }
}

/*
if(bodyguard != null && (bodyguard.IsShooting || bodyguard.IsWeaponReadyToShoot))
                    {
                        bodyguard.Tasks.Clear();
                        // debug msg
                        if (bodyguard.IsShooting || bodyguard.IsWeaponReadyToShoot)
                        {
                            Game.DisplayNotification("DEBUG: Something went wrong. Your partner should be following you, not aiming or shooting.");
                        }
                        else
                        {
                            Game.DisplayNotification("DEBUG: Success! Parnter has stopped aiming/shooting and should be following you now.");
                        }
                    }
                    else
                    {
                        Game.DisplayNotification("DEBUG: You partner is not (or shouldn't be) aiming/shooting at anyone! He should be following you now.");
                    }
                    */