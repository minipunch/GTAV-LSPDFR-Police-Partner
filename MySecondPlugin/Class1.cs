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
        /// <summary>
        /// Defines the entry point of the plugin.
        /// </summary>
        private static void Main()
        {
            // our bodyguard group
            Group bodyguardGroup = null;
            // our bodyguard
            Ped bodyguard = null;

            while (true)
            {
				// Key to spawn bodyguard (F9)
                if (Game.IsKeyDown(Keys.F9))
                {
					// Create bodyguard group
                    bodyguardGroup = Game.LocalPlayer.Character.Group;
					// set spawn locatation for bodyguard
                    Vector3 spawnLocation = Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront * 10f);
                    // spawn new bodyguard
					bodyguard = new Ped("S_M_Y_Sheriff_01", spawnLocation, 0f);
                    bodyguard.BlockPermanentEvents = true;
					// give him a weapon
                    NativeFunction.Natives.GiveWeaponToPed(bodyguard,0x1B06D571,450, true, true);
					// make him a cop
                    NativeFunction.Natives.SetPedAsCop(bodyguard, true);
					// display notification
                    Game.DisplayNotification("Bodyguard spawned!");
                }

				// if bodyguard exists, but for some reason is not in the player's group
                if (bodyguard != null && (bodyguardGroup.Count - 1) < 1)
                {
					// make sure he is always a member of the player group
                    bodyguardGroup.AddMember(bodyguard);
                }

                // If the bodyguard has been created, and your player is in a police car, but the bodyguard is not
                if (bodyguard != null && Game.LocalPlayer.Character.IsInAnyPoliceVehicle && !(bodyguard.IsInAnyPoliceVehicle))
                {

                    // set the proper car target for entry
                    Vehicle leaderVehicle = Game.LocalPlayer.Character.CurrentVehicle;

                    // move the bodyguard to the targetted car
                    NativeFunction.Natives.TaskGoToEntity(bodyguard, leaderVehicle, 5000, 2.0f, 200, 1073741824, 0);

                    // give some time for bodyguard to get to car before handling next condition
                    GameFiber.Wait(5000);

                    NativeFunction.Natives.TaskOpenVehicleDoor(bodyguard, leaderVehicle, 0, 1, 2.0f);
                    NativeFunction.Natives.TaskEnterVehicle(bodyguard, leaderVehicle, 1, 0, 2.0f, 1, 0);
                }

                // If the bodyguard is in a vehicle, while you are not
                if(bodyguard != null && bodyguard.IsInAnyPoliceVehicle && Game.LocalPlayer.Character.DistanceTo2D(bodyguard.GetOffsetPosition(Vector3.RelativeFront)) > 5f)
                {
					// make the bodyguard get out and follow you
                    NativeFunction.Natives.TaskLeaveVehicle(bodyguard, bodyguard.CurrentVehicle, 1);
                    NativeFunction.Natives.TaskGoToEntity(bodyguard, Game.LocalPlayer.Character, 5000, 2.0f, 200, 1073741824, 0);
                }

				// Yield to other plugins
                GameFiber.Yield();
            }

        }
    }
}