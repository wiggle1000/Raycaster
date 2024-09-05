using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class FileSelectorDialog
    {
        public string fileType = "lvl";
        public string rootDir = ".";
        public bool isFolderSelector = false;

        public bool wantsTypedName = false;
        public string typedName = "";

        public string cDir;
        private string[] cDirFiles;
        private string[] cDirDirs;

        public string selectedFile = null;
        public bool isDone = false;

        private float scroll = 0;
        private float scrollTarget = 0;
        private int hoveredIndex = 0;
        private bool cancelHovered = false;
        private bool nameHovered = false;

        private TextField nameText = new TextField(new Rectangle(0,0,0,0), "");

        private Rectangle cancelRect = new Rectangle(0, 0, 0, 0);

        public FileSelectorDialog(string rootDir, string fileType = "lvl", bool isFolderSelector = false, bool wantsTypedName = false)
        {
            this.fileType = fileType;
            this.rootDir = rootDir;
            this.isFolderSelector = isFolderSelector;
            this.wantsTypedName = wantsTypedName;
            cDir = rootDir;
            SelectDirectory(rootDir);
        }

        public bool CheckIfInRootDir(string dir)
        {
            bool isInRoot = false;
            string rootCheckDir = dir;
            for (int i = 0; i < 64; i++)
            {
                if (rootCheckDir == rootDir)
                    return true;
                DirectoryInfo di = Directory.GetParent(rootCheckDir);
                if (di == null || !di.Exists) return false;
                rootCheckDir = di.FullName;
            }
            return false;
        }

        public void SelectDirectory(string dir)
        {
            if(dir == "..")
            {
                dir = Directory.GetParent(cDir).FullName;
            }
            bool isInRoot = CheckIfInRootDir(dir);
            if (!isInRoot)
            {
                LevelEditor.EditorSounds["err"].Play(0.25f, 1f, 0);
                return;
            }

            LevelEditor.EditorSounds["chip3"].Play(0.25f, 1f, 0);


            cDir = dir;
            scrollTarget = 0;
            //sort files
            string[] d = Directory.GetFiles(dir);
            if(isFolderSelector)
            {
                cDirFiles = new string[]{"[SELECT CURRENT FOLDER]"};
            }
            else
            {
                List<string> filteredFiles = new List<string>();
                foreach (string thisFile in d)
                {
                    if (thisFile.ToLower().EndsWith(fileType.ToLower()))
                    {
                        filteredFiles.Add(thisFile);
                    }
                }
                cDirFiles = filteredFiles.ToArray();
            }
            d = Directory.GetDirectories(dir);
            cDirDirs = new string[d.Length + 1];
            cDirDirs[0] = "..";
            for(int i = 0; i < d.Length; i++)
            {
                cDirDirs[i + 1] = d[i];
            }
            Debug.Log("Loaded {0} subdirectories and {1} files in {2}", cDirDirs.Length, cDirFiles.Length, cDir);
        }

        private Rectangle? GetOptionRectangle(int index)
        {
            Rectangle rect = new Rectangle(30, 60 + (int)MathF.Floor(index*30 + (scroll+30)), Game._graphics.GraphicsDevice.ScissorRectangle.Width - 60, 30);
            float edgeNearness = 0;
            if (rect.Top < 120) edgeNearness = MathF.Pow(MathHelper.Clamp((120 - rect.Top) / 60f, 0, 1), 3);
            if (rect.Top < 120) rect.Y += (int)(edgeNearness * 30);
            int bottomY = Game._graphics.GraphicsDevice.ScissorRectangle.Height - 60;
            if (rect.Top > bottomY) edgeNearness = MathF.Pow(MathHelper.Clamp((rect.Top - bottomY) / 30f, 0, 1), 3);
            if (rect.Top > bottomY) rect.Y -= (int)(edgeNearness * 8);
            if (rect.Top > bottomY) rect.Height = (int)MathF.Floor(rect.Height * (1 - edgeNearness));
            if (edgeNearness >= 1) return null;
            return rect;
        }

        private string FixTypedName(string rawName)
        {
            string fixedName = rawName;
            foreach (char oopsie in Path.GetInvalidFileNameChars())
            {
                fixedName = fixedName.Replace(oopsie.ToString(), "");
            }
            return fixedName;
        }

        public void Update(float dt)
        {
            Rectangle scissorRect = Game._graphics.GraphicsDevice.ScissorRectangle;

            float lst = scrollTarget;
            scrollTarget -= InputManager.ScrollDelta * 4f;
            if(scrollTarget != lst)
            {
                LevelEditor.EditorSounds["tact2"].Play(0.25f, 1f, 0);
            }
            scroll = MathHelper.Lerp(scroll, scrollTarget, dt * 10f);
            bool isClicking = InputManager.IsMouseJustReleased(MouseButton.Left);

            //text box
            if (wantsTypedName)
            {
                nameText.bounds = new Rectangle(90, 50, scissorRect.Width - 100, 30);
                nameText.Update(dt);
                if (nameText.text != FixTypedName(nameText.text))
                {
                    nameText.text = FixTypedName(nameText.text);
                    LevelEditor.EditorSounds["err"].Play(0.25f, 1f, 0);
                }
            }
            
            //handle ok button (enter)
            if(wantsTypedName)
            {
                if (InputManager.KeyJustPressed(Keys.Enter))
                {
                    if (nameText.text != "")
                    {
                        nameText.selected = false;
                        typedName = nameText.text;
                        selectedFile = cDir;
                        isDone = true;
                        return;
                    }
                }
            }

            //handle cancel button (or escape)
            cancelRect = new Rectangle(scissorRect.Width - 120, 5, 100, 30);
            cancelHovered = cancelRect.Contains(InputManager.MousePos);
            if ((cancelRect.Contains(InputManager.MousePos) && isClicking) || InputManager.KeyDown(Keys.Escape))
            {
                isDone = true;
                selectedFile = null;
                LevelEditor.EditorSounds["err"].Play(0.25f, 1f, 0);
                return;
            }

            int optionIndex = 0;
            int lastHovered = hoveredIndex;
            for (int i = 0; i < cDirDirs.Length; i++)
            {
                Rectangle? rect = GetOptionRectangle(optionIndex);
                if (rect.HasValue && rect.Value.Contains(InputManager.MousePos))
                {
                    hoveredIndex = optionIndex;
                    if(hoveredIndex != lastHovered) LevelEditor.EditorSounds["tact4"].Play(0.25f, 1f, 0);
                    if (isClicking)
                    {
                        SelectDirectory(cDirDirs[i]);
                        return;
                    }
                    break;
                }
                optionIndex++;
            }
            for (int i = 0; i < cDirFiles.Length; i++)
            {
                Rectangle? rect = GetOptionRectangle(optionIndex);
                if (rect.HasValue && rect.Value.Contains(InputManager.MousePos))
                {
                    hoveredIndex = optionIndex;
                    if (isClicking)
                    {
                        if(isFolderSelector)
                        {
                            selectedFile = cDir;
                        }
                        else
                        {
                            selectedFile = cDirFiles[i];
                        }
                        if(wantsTypedName && FixTypedName(typedName) == "")
                        {
                            LevelEditor.EditorSounds["err"].Play(0.25f, 1f, 0);
                        }
                        else
                        {
                            typedName = FixTypedName(typedName);
                            isDone = true;
                            LevelEditor.EditorSounds["sel"].Play(0.25f, 1f, 0);
                        }
                    }
                    break;
                }
                optionIndex++;
            }
        }

        public void DrawFileList()
        {
            for (int i = 0; i < cDirDirs.Length + cDirFiles.Length; i++)
            {
                Rectangle? rectMaybe = GetOptionRectangle(i);
                if (!rectMaybe.HasValue) continue;
                Rectangle rect = rectMaybe.Value;
                bool isFile = (i >= cDirDirs.Length);
                Color itemColor = isFile ? Color.YellowGreen : Color.GreenYellow;
                if(hoveredIndex == i)
                {
                    itemColor = Color.Lerp(itemColor, Color.LightYellow, 0.5f);
                }
                Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Blank"], rect, itemColor);

                string label = isFile ? cDirFiles[i - cDirDirs.Length].Replace(cDir, "").Replace("\\","") : cDirDirs[i].Replace(cDir, ".");
                Game._spriteBatch.DrawString(Game.zector, label, rect.Location.ToVector2(), Color.Black);
            }
        }

        public void Draw()
        {
            Game._spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Stars"], Game._graphics.GraphicsDevice.ScissorRectangle, Color.LightGray);
            Game._spriteBatch.DrawString(Game.zector, isFolderSelector?"Select a folder":"Select a file to open", new Vector2(10, 5), Color.White);
            Game._spriteBatch.DrawString(Game.zectorTiny, cDir, new Vector2(10, 35), Color.White);
            //cancel button
            Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Blank"], cancelRect, cancelHovered ? Color.Plum : Color.Red);
            Game._spriteBatch.DrawString(Game.zector, "CANCEL", cancelRect.Location.ToVector2() + new Vector2(2, 0), Color.White);

            if (wantsTypedName)
            {
                Game._spriteBatch.DrawString(Game.zector, "Name:", nameText.bounds.Location.ToVector2() + new Vector2(-80, 0), Color.White);
                nameText.Draw();
            }

            DrawFileList();
            Game._spriteBatch.End();
        }
    }
}
