using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class TextField
    {
        public Rectangle bounds;
        public string text = "";

        Color bgColor = Color.DarkOrange;
        Color deselColor = Color.DarkViolet;
        Color hoverColor = Color.Violet;
        Color textColor = Color.White;

        private bool hovered = false;

        public bool selected = false;

        private float blinkTimer = 0;

        public TextField(Rectangle bounds, string defaultText = "")
        {
            this.bounds = bounds;
            this.text = defaultText;
        }

        public void Update(float dt)
        {
            hovered = bounds.Contains(Game.cMouseState.Position);
            bool clicking = Game.cMouseState.LeftButton == ButtonState.Released && Game.lastMouseState.LeftButton == ButtonState.Pressed;
            bool clicked = hovered && clicking;
            if (clicked)
            {
                selected = true;
                Game.window.TextInput += OnTextInput;
            }
            else if (clicking)
            {
                selected = false;
                Game.window.TextInput -= OnTextInput;
            }

            blinkTimer += dt;
            if (blinkTimer > 1)
            {
                blinkTimer = 0;
            }
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.Back)
            {
                if (this.text.Length > 0)
                {
                    this.text = this.text.Substring(0, this.text.Length - 1);
                }
            }
            else
            {
                if(e.Character > 20 && e.Character < 127)
                this.text += e.Character;
            }
        }

        public void Draw()
        {
            Game._spriteBatch.Draw(ResourceRegistry.TEXTURES["Blank"], bounds, hovered ? hoverColor : (selected ? bgColor : deselColor));
            string cursorString = (selected && blinkTimer > 0.5) ? "|" : "";
            Game._spriteBatch.DrawString(Game.zector, text+cursorString, bounds.Location.ToVector2() + new Vector2(2, 0), textColor);
        }
    }
}
