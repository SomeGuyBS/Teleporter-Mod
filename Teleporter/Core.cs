using Il2Cpp;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Teleporter.Core), "Teleporter", "1.3.0", "chris", null)]
[assembly: MelonGame("Sean Smith", "REPEATER")]

namespace Teleporter
{
    public class Core : MelonMod
    {
        private AssetBundle _bundle;
        private GameObject _handObject;
        private bool _firstPlayerUpdate; // Used on update to run once the same way as on scene loaded but with the player in the scene

        public override void OnInitializeMelon()
        {
            _bundle = AssetFromResource("Teleporter.teleporter");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                _firstPlayerUpdate = true;
            }

            // Checks if the asset bundle has been loaded
            if (_bundle)
            {

                // Checks if the player is not in the menu and that the Holder exists
                var teleporterHolder = GameObject.Find("Holder");
                if (teleporterHolder)
                {
                    var teleporterUI = _bundle.LoadAsset<GameObject>("Teleporter UI");
                    UnityEngine.Object.Instantiate(teleporterUI, teleporterHolder.transform);

                    var handObject = _bundle.LoadAsset<GameObject>("Teleporter Object Pfb");
                    _handObject = UnityEngine.Object.Instantiate(handObject);
                }
            }
            else
            {
                LoggerInstance.Error("Asset bundle not loaded");
            }
        }

        public override void OnUpdate()
        {
            if (_firstPlayerUpdate)
            {
                var gunHolder = GameObject.Find("Gun");
                if (gunHolder)
                {
                    _firstPlayerUpdate = false;
                    _handObject.transform.parent = gunHolder.transform;
                    _handObject.transform.localPosition = Vector3.zero;
                    _handObject.transform.localScale = Vector3.one;
                    _handObject.transform.localRotation = Quaternion.identity;
                    _handObject.active = false;
                }
            }

            // The keycodes for the weapons
            var k1 = Input.GetKeyDown(KeyCode.Alpha1);
            var k2 = Input.GetKeyDown(KeyCode.Alpha2);
            var k3 = Input.GetKeyDown(KeyCode.Alpha3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // Finds the player and the shooting script
                var weaponSelectionPanelObj = GameObject.Find("PlayerController(Clone)");
                var weaponSelectionPanel = weaponSelectionPanelObj.GetComponent<RaycastShooting>();

                // Disables gun firing and hides all other guns
                weaponSelectionPanel.ishunted = true;
                var g9 = GameObject.Find("Gun_v009");
                if (g9) g9.active = false;
                var p5 = GameObject.Find("Projectile_v005");
                if (p5) g9.active = false;
                var b4 = GameObject.Find("Bident_v004");
                if (b4) b4.active = false;
                
                // Enables the teleporter
                _handObject.active = true;
            }
            else if (k1 | k2 | k3)
            {
                // Finds the player and shooting script
                var weaponSelectionPanelObj = GameObject.Find("PlayerController(Clone)");
                var weaponSelectionPanel = weaponSelectionPanelObj.GetComponent<RaycastShooting>();

                // Re-enables shooting
                weaponSelectionPanel.ishunted = false;

                // Disables the teleporter
                _handObject.active = false;
            }
        }

        private static AssetBundle AssetFromResource(string name)
        {
            using (Stream? stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                MemoryStream? memory = new((int)stream.Length);
                stream!.CopyTo(memory);

                Il2CppSystem.IO.MemoryStream memoryStream = new Il2CppSystem.IO.MemoryStream(memory.ToArray());

                AssetBundle loadFromMemoryInternal = AssetBundle.LoadFromStream(memoryStream);
                return loadFromMemoryInternal;
            }
        }
    }
}