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

        /// <summary>
        /// Defines the entry point of the plugin.
        /// </summary>
        private static void Main()
        {
            while (true)
            {
                // Yield to other plugins
                GameFiber.Yield();
                // Key to spawn bodyguard (F9)
                if (Game.IsKeyDown(Keys.F9))
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

                    // OLD VERSION : NativeFunction.Natives.TaskGoToEntity(bodyguard, leaderVehicle, 5000, 2.0f, 200, 1073741824, 0);
                    // move the bodyguard to the targetted car
                    bodyguard.Tasks.GoToOffsetFromEntity(leaderVehicle, 0, 5.0f, 3.0f, 2.0f).WaitForCompletion();

                    // OLD VERSION: NativeFunction.Natives.TaskOpenVehicleDoor(bodyguard, leaderVehicle, 0, 1, 2.0f);
                    // enter the vehicle
                    bodyguard.Tasks.EnterVehicle(leaderVehicle, 0).WaitForCompletion();

                }

                // If the bodyguard is in a vehicle, while you are not
                if(bodyguard != null && bodyguard.IsInAnyPoliceVehicle && playerPed.DistanceTo2D(bodyguard.GetOffsetPosition(Vector3.RelativeFront)) > 5f)
                {
                    // OLD VERSION: NativeFunction.Natives.TaskLeaveVehicle(bodyguard, bodyguard.CurrentVehicle, 1);
                    // OLD VERSION: NativeFunction.Natives.TaskGoToEntity(bodyguard, playerPed, 5000, 2.0f, 200, 1073741824, 0);
                    // make the bodyguard get out and follow you
                    bodyguard.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                    bodyguard.Tasks.GoToOffsetFromEntity(playerPed, 0, 5.0f, 3.0f, 2.0f).WaitForCompletion();
                }

            }

        }
    }
}