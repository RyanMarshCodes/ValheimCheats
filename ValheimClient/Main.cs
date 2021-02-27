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
        public static Texture2D texture;
        private List<Player> players = new List<Player>();
        private List<MonsterAI> monsters = new List<MonsterAI>();
        private List<Container> containers = new List<Container>();
        private List<Trader> traders = new List<Trader>();
        private List<MineRock> mineRocks = new List<MineRock>();
        private List<MineRock5> mineRock5s = new List<MineRock5>();
        private List<Pickable> pickables = new List<Pickable>();
        private List<PickableItem> pickableItems = new List<PickableItem>();

        private string[] pickablesList = { "thistle" };

        private Vector3 savedLocation = new Vector3();
        private bool godMode = false;

        private bool _containerESP = false;
        private bool _playerESP = false;
        private bool _monsterESP = false;
        private bool _rockESP = false;
        private bool _pickableESP = false;

        private Camera camera;


        public void Start()
        {
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, ":)");

            camera = Camera.main;

            texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, Color.green);
            texture.SetPixel(1, 0, Color.green);
            texture.SetPixel(0, 1, Color.green);
            texture.SetPixel(1, 1, Color.green);
            texture.Apply();

            // Calls LoadObjects every second instead of every frame.
            InvokeRepeating("LoadObjects", 1f, 1f);
        }

        public void Update()
        {
            try
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
                    Loader.Unload();
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, ":(");
                }

                if (Input.GetKeyDown(KeyCode.Keypad8))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.transform.position += Player.m_localPlayer.transform.forward * 20f;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.transform.position += Player.m_localPlayer.transform.right * 20f;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.transform.position += Player.m_localPlayer.transform.right * -20f;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.transform.position += Player.m_localPlayer.transform.forward * -20f;
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

                if (Input.GetKey(KeyCode.Keypad0))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.Heal(Player.m_localPlayer.GetMaxHealth(), true);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Healed");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    if (Player.m_localPlayer)
                    {
                        Player.m_localPlayer.m_staminaRegen = Player.m_localPlayer.m_staminaRegen * 2f;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    // Save Location
                    if (Player.m_localPlayer)
                    {
                        savedLocation = Player.m_localPlayer.transform.position;
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Location saved!");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    if (Player.m_localPlayer && savedLocation != null)
                    {
                        // Teleport to saved location
                        float x = savedLocation.x;
                        float z = savedLocation.z;

                        Vector3 pos2 = new Vector3(x, Player.m_localPlayer.transform.position.y, z);
                        Player.m_localPlayer.TeleportTo(pos2, Player.m_localPlayer.transform.rotation, true);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    if (Player.m_localPlayer)
                    {
                        godMode = !godMode;
                        Player.m_localPlayer.SetGodMode(godMode);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "God Mode " + godMode.ToString());
                    }
                }

                if (Input.GetKeyDown(KeyCode.Keypad9))
                {
                    if (Player.m_localPlayer)
                    {
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Player.m_localPlayer.transform.position.ToString("F3"));
                    }
                }

                //if (Input.GetKeyDown(KeyCode.KeypadDivide))
                //{
                //    if (Player.m_localPlayer)
                //    {
                //        traders = GameObject.FindObjectsOfType<Trader>().ToList();

                //        if (traders?.Count > 0)
                //        {
                //            float x = traders.First().transform.position.x;
                //            float z = traders.First().transform.position.z;

                //            Vector3 pos2 = new Vector3(x, Player.m_localPlayer.transform.position.y, z);
                //            Player.m_localPlayer.TeleportTo(pos2, Player.m_localPlayer.transform.rotation, true);
                //            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Teleported to trader");
                //        }
                //        else
                //        {
                //            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "No trader found");
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, ex.Message);
            }

            //if (Input.GetKeyDown(KeyCode.Keypad5))
            //{
            //    string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            //    string savePath = desktopPath + "/ValheimLocations";
            //    string fileName = "/KiandeServer.txt";

            //    if (!Directory.Exists(savePath))
            //    {
            //        Directory.CreateDirectory(savePath);
            //    }

            //    if (Player.m_localPlayer)
            //    {
            //        string[] lines =
            //        {
            //                "--------------------------------------------",
            //                Player.m_localPlayer.transform.position.ToString("F0"),
            //                "--------------------------------------------"
            //            };

            //        File.AppendAllLines(savePath + fileName, lines);

            //        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Location Saved");
            //    }
            //    else
            //    {
            //        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "No localplayer");
            //    }
            //}
        }

        public void OnGUI()
        {
            if (_containerESP)
            {
                if (containers?.Count > 0)
                {
                    foreach (Container container in containers)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(container.transform.position);

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25 * 0.65f, 25f, texture, 1f);
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
                        Vector3 w2s = camera.WorldToScreenPoint(player.GetHeadPoint());

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible && !player.name.Contains("wae"))
                        {
                            float a = Math.Abs(camera.WorldToScreenPoint(player.GetHeadPoint()).y - camera.WorldToScreenPoint(player.transform.position).y);
                            DrawBox(w2s.x, Screen.height - w2s.y, a * 0.65f, a, texture, 1f);
                        }
                    }
                }
            }

            if (_monsterESP)
            {
                if (monsters?.Count > 0)
                {
                    foreach (MonsterAI monster in monsters)
                    {
                        Character character = monster.GetComponent<Character>();
                        Vector3 w2s = camera.WorldToScreenPoint(character.transform.position);

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            float a = Math.Abs(camera.WorldToScreenPoint(character.GetHeadPoint()).y - camera.WorldToScreenPoint(character.transform.position).y);
                            DrawBox(w2s.x, Screen.height - w2s.y, a * 0.65f, a, texture, 1f);
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

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25, 50, texture, 1f);
                        }
                    }
                }

                if (mineRock5s?.Count > 0) {

                    foreach (MineRock5 rock in mineRock5s)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(rock.transform.position);

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25, 50, texture, 1f);
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

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25, 50, texture, 1f);
                        }
                    }
                }

                if (pickableItems?.Count > 0)
                {

                    foreach (PickableItem item in pickableItems)
                    {
                        Vector3 w2s = camera.WorldToScreenPoint(item.transform.position);

                        bool IsVisible = w2s.z > -1;

                        if (IsVisible)
                        {
                            DrawBox(w2s.x, Screen.height - w2s.y, 25, 50, texture, 1f);
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
                    .Where(x => x.GetInventory().NrOfItems() > 0)
                    .ToList();
            }
            else
            {
                containers = new List<Container>();
            }

            if (_playerESP)
            {
                players = Player.GetAllPlayers()
                    .Where(x => x.name != Player.m_localPlayer.name)
                    .ToList();
            }
            else
            {
                players = new List<Player>();
            }


            if (_monsterESP)
            {
                monsters = GameObject.FindObjectsOfType<MonsterAI>()
                    .ToList();
            }
            else
            {
                monsters = new List<MonsterAI>();
            }

            if (_rockESP)
            {
                mineRocks = GameObject.FindObjectsOfType<MineRock>()
                    .Where(x => x.m_name.ToLower() != "rock")
                    .ToList();
                mineRock5s = GameObject.FindObjectsOfType<MineRock5>()
                    .Where(x => x.m_name.ToLower() != "rock")
                    .ToList();
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
                    .Where(x => pickablesList.Contains(!string.IsNullOrEmpty(x.m_overrideName) ? x.m_overrideName.ToLower() : x.GetHoverName().ToLower()))
                    .ToList();
                pickableItems = GameObject.FindObjectsOfType<PickableItem>()
                    .Where(x => FindDistanceFromPlayer(x.transform.position) < 100f)
                    .Where(x => pickablesList.Contains(x.GetHoverName().ToLower()))
                    .ToList();
            }
            else
            {
                pickables = new List<Pickable>();
                pickableItems = new List<PickableItem>();
            }
        }

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

        //private void HandleInput(string inputString)
        //{
        //    string[] parts = inputString.Split(' ');

        //    if (parts.Length > 0)
        //    {
        //        string command = parts[0];

        //        switch (command)
        //        {
        //            case "spawn":
        //                string searchItem = parts[1];

        //                int howMany = !String.IsNullOrEmpty(parts[2]) ? int.Parse(parts[2]) : 1;

        //                GameObject prefab = ZNetScene.instance.GetPrefab(searchItem);
        //                if (prefab)
        //                {
        //                    for (int i = 0; i < howMany; i++)
        //                    {
        //                        Character component2 = UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
        //                    }

        //                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Spawned " + parts[1]);
        //                }

        //                break;
        //            case "coords":
        //                if (Player.m_localPlayer)
        //                {
        //                    System.Console.WriteLine(Player.m_localPlayer.transform.position.ToString("F0"));
        //                }
        //                break;
        //            case "tp":
        //                if (Player.m_localPlayer)
        //                {

        //                }
        //                break;
        //            case "tp2":
        //                if (!string.IsNullOrEmpty(parts[1]))
        //                {
        //                    System.Console.WriteLine("you suck");
        //                }
        //                else
        //                {
        //                    string searchPlayer = parts[1];
        //                    Player player = Player.GetAllPlayers().FirstOrDefault(x => x.m_name.ToLower() == searchPlayer.ToLower());

        //                    if (player && Player.m_localPlayer)
        //                    {
        //                        Player.m_localPlayer.transform.position = player.transform.position;
        //                    }
        //                }
        //                break;
        //            case "exit":
        //                exit = true;
        //                break;
        //            default:
        //                System.Console.WriteLine("No command found for " + command);
        //                break;
        //        }
        //    }
        //}
    }
}
