using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimClient
{
    public class Main : MonoBehaviour
    {
        public static GUIStyle MenuItemStyle;

        public static Texture2D textureEnemy;
        public static Texture2D texturePlayer;
        public static Texture2D textureItem;
        public static GUIStyle enemyTextStyle;

        private bool menuOpen = false;
        private List<Player> players = new List<Player>();
        private List<MonsterAI> monsters = new List<MonsterAI>();
        private List<Container> containers = new List<Container>();
        private List<Trader> traders = new List<Trader>();
        private List<MineRock> mineRocks = new List<MineRock>();
        private List<MineRock5> mineRock5s = new List<MineRock5>();
        private List<Pickable> pickables = new List<Pickable>();
        private List<PickableItem> pickableItems = new List<PickableItem>();

        private List<Vegvisir> bossStones = new List<Vegvisir>();

        private string[] pickablesList = { "thistle" };

        private Vector3 savedLocation = new Vector3();
        private bool godMode = false;
        private Player localPlayer;

        private bool _containerESP = false;
        private bool _playerESP = false;
        private bool _monsterESP = false;
        private bool _rockESP = false;
        private bool _pickableESP = false;

        private Camera camera;

        public Smelter smelter1;
        public bool smelterHack = false;
        public int m_maxOre = 10;
        public int m_maxFuel = 10;
        public int m_fuelPerProduct = 1;
        public float m_secPerProduct = 10f;

        public float m_fermentationDuration = 2400f;

        public void Start()
        {
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, ":)");

            camera = Camera.main;
            localPlayer = Player.m_localPlayer;

            textureEnemy = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            textureEnemy.SetPixel(0, 0, UnityEngine.Color.red);
            textureEnemy.SetPixel(1, 0, UnityEngine.Color.red);
            textureEnemy.SetPixel(0, 1, UnityEngine.Color.red);
            textureEnemy.SetPixel(1, 1, UnityEngine.Color.red);
            textureEnemy.Apply();

            texturePlayer = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texturePlayer.SetPixel(0, 0, UnityEngine.Color.white);
            texturePlayer.SetPixel(1, 0, UnityEngine.Color.white);
            texturePlayer.SetPixel(0, 1, UnityEngine.Color.white);
            texturePlayer.SetPixel(1, 1, UnityEngine.Color.white);
            texturePlayer.Apply();

            textureItem = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            textureItem.SetPixel(0, 0, UnityEngine.Color.green);
            textureItem.SetPixel(1, 0, UnityEngine.Color.green);
            textureItem.SetPixel(0, 1, UnityEngine.Color.green);
            textureItem.SetPixel(1, 1, UnityEngine.Color.green);
            textureItem.Apply();

            //enemyTextStyle = new GUIStyle(GUI.skin.label);
            //enemyTextStyle.fontSize = 12;
            //enemyTextStyle.alignment = TextAnchor.MiddleCenter;
            //enemyTextStyle.normal.textColor = Color.red;

            // Calls LoadObjects every second instead of every frame.
            InvokeRepeating("LoadObjects", 1f, 1f);
        }

        public void OnDestroy()
        {
            Loader.Unload();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _containerESP = !_containerESP;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Containers: " + _containerESP.ToString());
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                _playerESP = !_playerESP;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Players: " + _playerESP.ToString());
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                _monsterESP = !_monsterESP;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Monsters: " + _monsterESP.ToString());
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                smelterHack = !smelterHack;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Smelters: " + smelterHack.ToString());

                List<Smelter> smelters = GameObject.FindObjectsOfType<Smelter>().ToList();

                foreach (Smelter smelter in smelters)
                {
                    if (smelter.name.ToLower().Contains("kiln"))
                    {
                        smelter.m_secPerProduct = smelterHack ? 0.1f : 5;
                    }
                    else
                    {
                        smelter.m_maxOre = smelterHack ? 100 : m_maxOre;
                        smelter.m_fuelPerProduct = smelterHack ? 0 : m_fuelPerProduct;
                        smelter.m_secPerProduct = smelterHack ? 0.1f : m_secPerProduct;
                    }
                }

                List<Fermenter> fermenters = GameObject.FindObjectsOfType<Fermenter>().ToList();

                foreach (Fermenter fermenter in fermenters)
                {
                    fermenter.m_fermentationDuration = smelterHack ? 0.1f : m_fermentationDuration;
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (localPlayer)
                {
                    localPlayer.transform.position += localPlayer.transform.forward * 20f;

                    Ship playerShip = localPlayer.GetControlledShip();

                    if (playerShip)
                    {
                        playerShip.transform.position += playerShip.transform.forward * 20f;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (localPlayer)
                {
                    List<Tameable> wolves = GameObject.FindObjectsOfType<Tameable>()
                        .Where(x => x.name.ToLower().StartsWith("wolf"))
                        .Where(x => x.IsHungry())
                        .ToList();

                    GameObject rawMeat = ZNetScene.instance.GetPrefab("RawMeat");

                    if (rawMeat)
                    {
                        foreach (Tameable wolf in wolves)
                        {
                                Vector3 b = UnityEngine.Random.insideUnitSphere * 0.5f;
                                localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning raw meat near wolf " + wolf.name, 0, null);
                                Character component3 = UnityEngine.Object.Instantiate<GameObject>(rawMeat, wolf.transform.position + wolf.transform.forward * 1f + Vector3.up + b, Quaternion.identity).GetComponent<Character>();
                        }
                    }

                    List<Tameable> boars = GameObject.FindObjectsOfType<Tameable>()
                        .Where(x => x.name.ToLower().StartsWith("boar"))
                        .Where(x => x.IsHungry())
                        .ToList();

                    GameObject carrot = ZNetScene.instance.GetPrefab("Carrot");

                    if (carrot)
                    {
                        foreach (Tameable boar in boars)
                        {
                            Vector3 b = UnityEngine.Random.insideUnitSphere * 0.5f;
                            localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning carrot near boar " + boar.name, 0, null);
                            Character component3 = UnityEngine.Object.Instantiate<GameObject>(carrot, boar.transform.position + boar.transform.forward * 1f + Vector3.up + b, Quaternion.identity).GetComponent<Character>();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("ArrowPoison", 100, 10, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    playerInventory.AddItem("ArrowFrost", 100, 10, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    //playerInventory.AddItem("ArrowFire", 100, 10, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    // playerInventory.AddItem("ArrowWood", 100, 10, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());

                    // playerInventory.AddItem("GreydwarfEye", 20, 10, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("LoxPie", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    playerInventory.AddItem("FishWraps", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    playerInventory.AddItem("SerpentStew", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());


                    // playerInventory.AddItem("Muckshake", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    // playerInventory.AddItem("Sausages", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                    // playerInventory.AddItem("QueensJam", 10, 1, 0, localPlayer.GetPlayerID(), localPlayer.GetPlayerName());
                }
            }

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                _rockESP = !_rockESP;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Rocks: " + _rockESP.ToString());
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                _pickableESP = !_pickableESP;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Pickables: " + _pickableESP.ToString());
            }
            
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                // Save Location
                if (localPlayer)
                {
                    savedLocation = localPlayer.transform.position;
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Location saved!");
                }

                //this.menuOpen = !this.menuOpen;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (localPlayer && savedLocation != null)
                {
                    // Teleport to saved location
                    float x = savedLocation.x;
                    float z = savedLocation.z;

                    float distance = Vector3.Distance(savedLocation, localPlayer.transform.position);

                    localPlayer.Message(MessageHud.MessageType.Center, "Teleporting " + distance.ToString("F1"));

                    Vector3 pos2 = new Vector3(x, localPlayer.transform.position.y, z);
                    localPlayer.TeleportTo(pos2, localPlayer.transform.rotation, distance > 200f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
               if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();
                    playerInventory.AddItem("FineWood", 50, 1, 0, 0, "");
                    playerInventory.AddItem("ElderBark", 50, 1, 0, 0, "");
                    playerInventory.AddItem("RoundLog", 50, 1, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("IronScrap", 50, 1, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                if (localPlayer)
                {
                    godMode = !godMode;
                    localPlayer.SetGodMode(godMode);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "God Mode " + godMode.ToString());

                    Player.m_debugMode = godMode;
                    localPlayer.SetPrivateField("m_noPlacementCost", godMode);
                    localPlayer.m_staminaRegenDelay = godMode ? 0.1f : 1f;
                    localPlayer.m_staminaRegen = godMode ? 99f : 5f;
                    localPlayer.m_runStaminaDrain = godMode ? 0f : 10f;

                    localPlayer.SetMaxStamina(godMode ? 999f : 100f, true);

                    localPlayer.m_maxCarryWeight = godMode ? 9999f : 300f;

                    

                    localPlayer.SetGhostMode(godMode);
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("Stone", 50, 1, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("Wood", 50, 1, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();

                    playerInventory.AddItem("Coal", 50, 1, 0, 0, "");
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                if (localPlayer)
                {
                    Inventory playerInv = localPlayer.GetInventory();

                    if (playerInv != null)
                    {
                        List<ItemDrop.ItemData> items = playerInv.GetAllItems();

                        if (items?.Count > 0)
                        {
                            foreach (ItemDrop.ItemData item in items)
                            {
                                if (item.IsEquipable())
                                {
                                    if (item.GetDurabilityPercentage() != 1f)
                                    {
                                        item.m_durability = item.GetMaxDurability();
                                    }
                                }
                            }
                        }
                    }

                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "items repaired?");
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadPeriod))
            {
                if (localPlayer)
                {
                    Inventory playerInventory = localPlayer.GetInventory();
                    playerInventory.AddItem("GreydwarfEye", 10, 0, 0, 0, "");
                    playerInventory.AddItem("SurtlingCore", 2, 0, 0, 0, "");
                    playerInventory.AddItem("FineWood", 20, 1, 0, 0, "");
                }
            }
        }

        public void OnGUI()
        {
            if (this.menuOpen)
            {
                GUI.Box(new Rect(20, 50, 170, 150), "Wae Mod Menu");
            }

            if (_containerESP)
            {
                if (containers?.Count > 0)
                {
                    foreach (Container container in containers)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(container.transform.position);

                        Inventory containerInventory = container.GetPrivateField<Inventory>("m_inventory");

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25, 10, textureItem);
                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), String.Format("{0}/{1} Items ({2} ft.)", containerInventory.GetAllItems().Count, (containerInventory.GetWidth() * containerInventory.GetHeight()), Vector3.Distance(localPlayer.transform.position, container.transform.position).ToString("F1")), textStyle);
                        }
                    }
                }
            }

            if (_playerESP)
            {
                if (players?.Count > 0)
                {
                    foreach (Player player in players)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(player.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 20, texturePlayer);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.white;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), player.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, player.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }
            }

            if (_monsterESP)
            {
                if (monsters?.Count > 0)
                {
                    foreach (BaseAI monster in monsters)
                    {
                        Character character = monster.GetPrivateField<Character>("m_character");

                        if (character.m_health > 0)
                        {
                            Vector3 w2s = camera.WorldToScreenPoint(monster.transform.position);
                            if (w2s.z > -1)
                            {

                                DrawBox(w2s.x, Screen.height - w2s.y, 10, 20, textureEnemy);

                                var textStyle = new GUIStyle(GUI.skin.label);
                                textStyle.fontSize = 12;
                                textStyle.alignment = TextAnchor.MiddleCenter;
                                textStyle.normal.textColor = Color.red;
                                GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), character.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, monster.transform.position).ToString("F1") + " ft.)", textStyle);
                            }
                        }
                    }
                }
            }

            if (_rockESP)
            {
                if (mineRocks?.Count > 0)
                {
                    foreach (MineRock rock in mineRocks)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(rock.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 10, textureItem);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), rock.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, rock.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }

                if (mineRock5s?.Count > 0)
                {

                    foreach (MineRock5 rock in mineRock5s)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(rock.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 10, textureItem);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), rock.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, rock.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }

                if (bossStones?.Count > 0)
                {
                    foreach (Vegvisir stone in bossStones)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(stone.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 10, textureItem);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), stone.m_locationName + " (" + Vector3.Distance(localPlayer.transform.position, stone.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }
            }

            if (_pickableESP)
            {
                if (pickables?.Count > 0)
                {
                    foreach (Pickable item in pickables)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(item.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 10, textureItem);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), item.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, item.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }

                if (pickableItems?.Count > 0)
                {

                    foreach (PickableItem item in pickableItems)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(item.transform.position);

                        if (w2s.z > -1)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 10, 10, textureItem);

                            var textStyle = new GUIStyle(GUI.skin.label);
                            textStyle.fontSize = 12;
                            textStyle.alignment = TextAnchor.MiddleCenter;
                            textStyle.normal.textColor = Color.green;
                            GUI.Label(new Rect(w2s.x - 150, Screen.height - w2s.y, 300, 25), item.GetHoverName() + " (" + Vector3.Distance(localPlayer.transform.position, item.transform.position).ToString("F1") + " ft.)", textStyle);
                        }
                    }
                }
            }
        }

        private void LoadObjects()
        {
            if (_containerESP)
            {
                containers = GameObject.FindObjectsOfType<Container>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.GetInventory().NrOfItems() > 0)
                    .ToList();
            }
            else
            {
                containers = new List<Container>();
            }

            if (_playerESP)
            {
                players = GameObject.FindObjectsOfType<Player>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.GetPlayerName() != localPlayer.GetPlayerName())
                    .ToList();
            }
            else
            {
                players = new List<Player>();
            }


            if (_monsterESP)
            {
                monsters = GameObject.FindObjectsOfType<MonsterAI>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .ToList();
            }
            else
            {
                monsters = new List<MonsterAI>();
            }

            if (_rockESP)
            {
                mineRocks = GameObject.FindObjectsOfType<MineRock>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.m_name.ToLower() != "rock")
                    .ToList();
                mineRock5s = GameObject.FindObjectsOfType<MineRock5>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.m_name.ToLower() != "rock")
                    .ToList();
                bossStones = GameObject.FindObjectsOfType<Vegvisir>().ToList();
            }
            else
            {
                mineRocks = new List<MineRock>();
                mineRock5s = new List<MineRock5>();
            }

            if (_pickableESP)
            {
                pickables = GameObject.FindObjectsOfType<Pickable>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.GetHoverName().ToLower().Contains("thistle") || x.GetHoverName().ToLower().Contains("turnip"))
                    .ToList();
                pickableItems = GameObject.FindObjectsOfType<PickableItem>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => x.GetHoverName().ToLower().Contains("thistle") || x.GetHoverName().ToLower().Contains("turnip"))
                    .ToList();
            }
            else
            {
                pickables = new List<Pickable>();
                pickableItems = new List<PickableItem>();
            }
        }

        //private void ShowMainMenu(int windowID)
        //{
        //    GUIStyle guiStyle = new GUIStyle(GUI.skin.box);
        //    GUILayout.BeginVertical(new GUILayoutOption[0]);

        //    var menuOptions = new List<MenuOption>
        //    {
        //        new MenuOption { Command = "/test", Description = "Test command", subMenuOptions =  new List<MenuOption> { } }
        //    };

        //    foreach (MenuOption option in menuOptions)
        //    {
        //        //if (selectedOption == option)
        //        //{
        //        //    GUI.color = Color.white;
        //        //}
        //        GUILayout.Label(option.Description, guiStyle, new GUILayoutOption[0]);
        //    }
        //}

        private void DrawBox(float x, float y, float width, float height, Texture2D text, float thickness = 1f)
        {
            DrawRectangleOutlined(x - width / 2f, y - height, width, height, text, thickness);
        }

        private void DrawRectangleOutlined(float x, float y, float width, float height, Texture2D text, float thickness = 1) 
        {
            DrawRectangleFilled(x, y, thickness, height, text);
            DrawRectangleFilled(x + width - thickness, y, thickness, height, text);
            DrawRectangleFilled(x + thickness, y, width - thickness * 2f, thickness, text);
            DrawRectangleFilled(x + thickness, y + height - thickness, width - thickness * 2f, thickness, text);
        }

        private void DrawRectangleFilled(float x, float y, float width, float height, Texture2D text)
        {
            GUI.DrawTexture(new Rect(x, y, width, height), text);
        }

        private float FindDistanceFromPlayer(Vector3 position)
        {
            return Vector3.Distance(GameCamera.instance.transform.position, position);
        }
    }
}
