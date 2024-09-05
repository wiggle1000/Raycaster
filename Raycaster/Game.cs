using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Raycaster
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static SpriteFont zector;
        public static SpriteFont zectorTiny;

        public bool isLevelEditor = true;

        internal LevelEditor editor = new LevelEditor();
        internal LevelImplementation testLevel = LevelImplementation.LoadLevel("test.lvl");
        internal RaycastRender renderer = new RaycastRender();

        

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceRegistry.RegisterTexture(Content, "Blank");
            ResourceRegistry.RegisterTexture(Content, "Clear");
            ResourceRegistry.RegisterTexture(Content, "Grid");
            ResourceRegistry.RegisterTexture(Content, "Stars");
            ResourceRegistry.RegisterTexture(Content, "25Percent");
            ResourceRegistry.RegisterTexture(Content, "50Percent");
            ResourceRegistry.RegisterTexture(Content, "75Percent");
            ResourceRegistry.RegisterTexture(Content, "Wall");
            ResourceRegistry.RegisterTexture(Content, "Floor");
            ResourceRegistry.RegisterTexture(Content, "Roof");
            ResourceRegistry.RegisterTexture(Content, "Obj1");
            ResourceRegistry.RegisterTexture(Content, "Obj2");
            ResourceRegistry.RegisterTexture(Content, "Obj3");
            ResourceRegistry.RegisterTexture(Content, "Obj4");
            zector = Content.Load<SpriteFont>("Zector");
            zectorTiny = Content.Load<SpriteFont>("ZectorTiny");
            editor.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isLevelEditor)
            {
                editor.Update(dt);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if(isLevelEditor)
            {
                editor.Draw();
            }

            base.Draw(gameTime);
        }
    }
}