using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.Reflection.Metadata;
using System.IO;

namespace Raycaster
{
    internal class LevelEditor
    {
        public LevelImplementation editingLevel = new LevelImplementation();

        public static Dictionary<String, SoundEffect> EditorSounds = new Dictionary<string, SoundEffect>();
        private FileSelectorDialog openDialog;

        public bool isOpeningFile = false;
        public bool isSavingFile = false;

        public LevelImplementation.LayerType cLayer = LevelImplementation.LayerType.floor;
        public int settingTileTo = 0;
        bool cursorOverMap = false;
        int hoveredX, hoveredY = 0;

        float tileDisplaySize = 60;
        float offsetX = 0;
        float offsetY = 0;
        float offsetVelX = 0;
        float offsetVelY = 0;

        float moveSpeedTarget = 10;
        float moveSoundCounter = 0;

        int lastScrollValue = 0;


        private void LoadSound(ContentManager Content, string name)
        {
            EditorSounds.Add(name, Content.Load<SoundEffect>("SFX/Editor/" + name));
        }
        public void LoadContent(ContentManager Content)
        {
            if(EditorSounds.Count == 0)
            {
                LoadSound(Content, "tact1");
                LoadSound(Content, "tact2");
                LoadSound(Content, "tact3");
                LoadSound(Content, "tact4");
                LoadSound(Content, "tact5");

                LoadSound(Content, "chip1");
                LoadSound(Content, "chip2");
                LoadSound(Content, "chip3");
                LoadSound(Content, "err");
                LoadSound(Content, "sel");

                LoadSound(Content, "layer0");
                LoadSound(Content, "layer1");
                LoadSound(Content, "layer2");
                LoadSound(Content, "layer3");
                LoadSound(Content, "layer4");
            }
        }

        public void Update(float dt)
        {
            if (isOpeningFile)
            {
                openDialog.Update(dt);
                if (openDialog.isDone)
                {
                    if (openDialog.selectedFile == null) return;
                    Open(openDialog.selectedFile);
                }
                return;
            }
            else if (isSavingFile)
            {
                openDialog.Update(dt);
                if (openDialog.isDone)
                {
                    if (openDialog.selectedFile == null) return;
                    //TODO: ASK FOR SAVE FILE NAME!!
                    Save(openDialog.selectedFile);
                }
                return;
            }

            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            //get tile hovered over
            hoveredX = (int)MathF.Floor(ms.X / tileDisplaySize - offsetX);
            hoveredY = (int)MathF.Floor(ms.Y / tileDisplaySize - offsetY);
            cursorOverMap = hoveredX >= 0 && hoveredX < editingLevel.width && hoveredY >= 0 && hoveredY < editingLevel.width;

            //horizontal, vertical movement
            float moveAccelSpeed = dt * 10;

            if (ks.IsKeyDown(Keys.A)) offsetVelX = MathHelper.Lerp(offsetVelX, moveSpeedTarget, moveAccelSpeed);
            else if (ks.IsKeyDown(Keys.D)) offsetVelX = MathHelper.Lerp(offsetVelX, -moveSpeedTarget, moveAccelSpeed);
            else { offsetVelX = MathHelper.Lerp(offsetVelX, 0, moveAccelSpeed); if (MathF.Abs(offsetVelX) < 0.1f) offsetVelX = 0; }

            if (ks.IsKeyDown(Keys.W)) offsetVelY = MathHelper.Lerp(offsetVelY, moveSpeedTarget, moveAccelSpeed);
            else if (ks.IsKeyDown(Keys.S)) offsetVelY = MathHelper.Lerp(offsetVelY, -moveSpeedTarget, moveAccelSpeed);
            else { offsetVelY = MathHelper.Lerp(offsetVelY, 0, moveAccelSpeed); if (MathF.Abs(offsetVelY) < 0.1f) offsetVelY = 0; }

            offsetX += offsetVelX * dt;
            offsetY += offsetVelY * dt;

            //--------HOTKEYS--------

            //ctrl+...
            if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
            {
                if (ks.IsKeyDown(Keys.O))
                {
                    openDialog = new FileSelectorDialog(Directory.GetCurrentDirectory());
                    isOpeningFile = true;
                }
                if (ks.IsKeyDown(Keys.S))
                {
                    openDialog = new FileSelectorDialog(Directory.GetCurrentDirectory(),"", true);
                    isSavingFile = true;
                }
            }

            //--------zooming, layer scrolling, tile id change--------
            int scroll = (int)MathF.Round(MathHelper.Clamp((ms.ScrollWheelValue - lastScrollValue)/100f, -15, 15));
            if(scroll != 0)
            {
                //SWITCHING PALETTE
                if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
                {
                    settingTileTo = MathHelper.Clamp(settingTileTo + scroll, 0, 32);
                    if (scroll > 0)
                        EditorSounds["chip2"].Play(0.25f, 0.5f, 0);
                    else
                        EditorSounds["chip1"].Play(0.25f, 0.5f, 0);
                }
                //SWITCHING LAYERS
                else if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
                {
                    cLayer = (LevelImplementation.LayerType)MathHelper.Clamp((byte)cLayer + scroll, 0, 4);
                    if (EditorSounds.ContainsKey("layer" + (byte)cLayer)) EditorSounds["layer" + (byte)cLayer].Play(0.25f, 1f, 0);
                }
                //ZOOMING
                else
                {
                    tileDisplaySize = MathHelper.Clamp(tileDisplaySize + scroll, 32, 120);
                    if (scroll > 0)
                        EditorSounds["chip2"].Play(0.25f, 1f, 0);
                    else
                        EditorSounds["chip1"].Play(0.25f, 1f, 0);
                }
                lastScrollValue = ms.ScrollWheelValue;
            }

            //--------placing tiles--------
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (cursorOverMap)
                {
                    if (editingLevel.GetMapLayerByLayerType(cLayer).GetAt(hoveredX, hoveredY) != settingTileTo)
                    {
                        editingLevel.GetMapLayerByLayerType(cLayer).SetAt(hoveredX, hoveredY, settingTileTo);
                        Debug.Log("Setting ({0}, {1}) to {2}", hoveredX, hoveredY, settingTileTo);
                        EditorSounds["tact5"].Play(0.25f, 1f, 0);
                    }
                }
            }
            //--------erasing tiles--------
            else if (ms.RightButton == ButtonState.Pressed)
            {
                if (cursorOverMap)
                {
                    if (editingLevel.GetMapLayerByLayerType(cLayer).GetAt(hoveredX, hoveredY) != 0)
                    {
                        editingLevel.GetMapLayerByLayerType(cLayer).SetAt(hoveredX, hoveredY, 0);
                        Debug.Log("Setting ({0}, {1}) to {2}", hoveredX, hoveredY, 0);
                        EditorSounds["tact1"].Play(0.25f, 1f, 0);
                    }
                }
            }
            //--------picking tile from map--------
            else if (ms.MiddleButton == ButtonState.Pressed)
            {
                if (cursorOverMap)
                {
                    if (editingLevel.GetMapLayerByLayerType(cLayer).GetAt(hoveredX, hoveredY) != settingTileTo)
                    {
                        settingTileTo = editingLevel.GetMapLayerByLayerType(cLayer).GetAt(hoveredX, hoveredY);
                        Debug.Log("Picking # {2} from ({0}, {1}) ", hoveredX, hoveredY, settingTileTo);
                        EditorSounds["tact4"].Play(0.3f, 1f, 0);
                    }
                }
            }

            //--------movement sounds--------
            float soundMoveSpeed = MathHelper.Clamp(new Vector2(offsetVelX * dt, offsetVelY * dt).Length()*4f, 0, 1);
            if (moveSoundCounter > 4f)
            {
                EditorSounds["tact1"].Play(MathHelper.Clamp(soundMoveSpeed/4, 0, 0.15f), MathHelper.Clamp(soundMoveSpeed, 0.25f, 1), 0);
                moveSoundCounter = 0;
            }
            moveSoundCounter += soundMoveSpeed;
        }

        private void DrawFadedLayer(MapLayer layer, Color color, string texName = "50Percent")
        {
            for (int x = 0; x < editingLevel.width; x++)
            {
                for (int y = 0; y < editingLevel.height; y++)
                {
                    int tileID = layer.GetAt(x, y);
                    Rectangle tileRect = new Rectangle(
                        (int)MathF.Floor((x + offsetX) * tileDisplaySize),
                        (int)MathF.Floor((y + offsetY) * tileDisplaySize),
                        (int)tileDisplaySize,
                        (int)tileDisplaySize
                        );
                    //Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Grid"], tileRect, (cursorOverMap && hoveredX == x && hoveredY == y) ? Color.HotPink : Color.Aqua);
                    if (tileID != 0)
                    {
                        Game._spriteBatch.Draw(ResourceRegistry.TEXTURES[texName], tileRect, color);
                    }
                }
            }
        }

        private void DrawLayer(MapLayer layer, string texName = "Blank")
        {
            for (int x = 0; x < editingLevel.width; x++)
            {
                for (int y = 0; y < editingLevel.height; y++)
                {
                    int tileID = layer.GetAt(x, y);
                    Rectangle tileRect = new Rectangle(
                        (int)MathF.Floor((x + offsetX) * tileDisplaySize),
                        (int)MathF.Floor((y + offsetY) * tileDisplaySize),
                        (int)tileDisplaySize,
                        (int)tileDisplaySize
                        );
                    Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Grid"], tileRect, (cursorOverMap && hoveredX == x && hoveredY == y)?Color.HotPink : Color.Aqua);
                    if (tileID != 0)
                    {
                        Game._spriteBatch.Draw(ResourceRegistry.TEXTURES[texName], tileRect, Color.SlateBlue);
                    }
                    Game._spriteBatch.DrawString(
                        Game.zector,
                        tileID.ToString(),
                        tileRect.Location.ToVector2() + new Vector2(tileRect.Width / 10, tileRect.Height / 10),
                        Color.LightYellow,
                        0, Vector2.Zero,
                        MathF.Max((tileDisplaySize / 60f), 0.8f),
                        SpriteEffects.None, 0);
                }
            }
        }

        public void Draw()
        {
            if (isOpeningFile || isSavingFile)
            {
                openDialog.Draw();
                return;
            }
            Game._spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Stars"], Game._graphics.GraphicsDevice.ScissorRectangle, Color.DarkSlateGray);
            if (cLayer == LevelImplementation.LayerType.floor)
            {
                DrawFadedLayer(editingLevel.floorLayer, Color.SlateBlue, "Floor");
                DrawFadedLayer(editingLevel.wallLayer, Color.Wheat, "Wall");
                DrawFadedLayer(editingLevel.markerLayer, Color.GreenYellow, "Obj1");
                DrawLayer(editingLevel.GetMapLayerByLayerType(cLayer), "Clear");
            }
            else if (cLayer == LevelImplementation.LayerType.wall)
            {
                DrawFadedLayer(editingLevel.floorLayer, Color.DarkSlateGray, "Floor");
                DrawLayer(editingLevel.GetMapLayerByLayerType(cLayer));
            }
            else if (cLayer == LevelImplementation.LayerType.ceil)
            {
                DrawFadedLayer(editingLevel.floorLayer, Color.DarkSlateGray, "Floor");
                DrawFadedLayer(editingLevel.wallLayer, Color.Wheat, "Wall");
                DrawLayer(editingLevel.GetMapLayerByLayerType(cLayer), "Roof");
            }
            else if (cLayer == LevelImplementation.LayerType.marker)
            {
                DrawFadedLayer(editingLevel.floorLayer, Color.DarkSlateGray, "Floor");
                DrawFadedLayer(editingLevel.wallLayer, Color.Wheat, "Wall");
                DrawFadedLayer(editingLevel.ceilLayer, Color.SlateGray, "Roof");
                DrawLayer(editingLevel.GetMapLayerByLayerType(cLayer), "Obj1");
            }
            else if (cLayer == LevelImplementation.LayerType.meta)
            {
                DrawFadedLayer(editingLevel.floorLayer, Color.DarkSlateGray, "Floor");
                DrawFadedLayer(editingLevel.wallLayer, Color.Wheat, "Wall");
                DrawFadedLayer(editingLevel.ceilLayer, Color.SlateGray, "Roof");
                DrawFadedLayer(editingLevel.markerLayer, Color.GreenYellow, "Obj1");
                DrawLayer(editingLevel.GetMapLayerByLayerType(cLayer), "Obj2");
            }
            Game._spriteBatch.DrawString(Game.zector, "Editing:" + ((int)cLayer) + " - " + cLayer, new Vector2(10, 5), Color.White);
            Game._spriteBatch.DrawString(Game.zector, "Setting:" + settingTileTo + " - " + "(TILE_NAME)", new Vector2(10, 50), Color.White);
            Game._spriteBatch.End();
        }

        public void Save(string path = "EDITOR_SAVED/untitled.lvl")
        {
            LevelSerializerUtils.SaveLevel(editingLevel, path);
        }

        public void Open(string path = "EDITOR_SAVED/untitled.lvl")
        {
            editingLevel = LevelSerializerUtils.LoadLevel(path);
        }
    }
}
