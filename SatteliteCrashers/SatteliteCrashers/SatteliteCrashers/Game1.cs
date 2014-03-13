using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;


namespace SatteliteCrashers
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    enum GameState
    {
        playing,
        start
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        RenderHelper renderHelper;
        SpriteBatch spriteBatch;

        GameState gameState;

        Model desModel;
        Matrix desWorldMat;

        Vector3 desDiff = new Vector3(0.997f, 0.377f, 0.787f);
        //Vector3 desSpec = new Vector3(255f / 255f, 204f / 255, 229f / 255);
        Vector3 desSpec = Vector3.Zero;
        Vector3 desAmb = .5f * new Vector3(0.997f, 0.377f, 0.787f);

        Texture2D startScreen;
        Texture2D startNoSelect;
        Texture2D startSelect;
        Texture2D exitNoSelect;
        Texture2D exitSelect;
        float startScale = 1;

        public static int markforwin;

        bool startSelected = true;

        public Game1()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1280;
            graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Data.clouds = new List<Cloud>();
            Data.stars = new List<PlaneTexture>();
            gameState = GameState.start;
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        Song fly;
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Effect renderEffect = Content.Load<Effect>("3dRender");
            Model playermodel = Content.Load<Model>("shredgodpose");
            Model rampModel = Content.Load<Model>("bowl");
            desModel = Content.Load<Model>("desert");
            Texture2D sunF1 = Content.Load<Texture2D>("sun1");
            Texture2D sunF2 = Content.Load<Texture2D>("sun2");
            fly = Content.Load<Song>("flylike");
            

            ScoreBoard.Init(Content, graphics.GraphicsDevice.Viewport);

            SoundEffect skate = Content.Load<SoundEffect>("skate");

            var sunPT = new PlaneTexture(sunF1, new Vector3(0, -10000, 0), 8, graphics.GraphicsDevice);
            var sunFrames = new List<Texture2D>();
            sunFrames.Add(sunF1);
            sunFrames.Add(sunF2);
            Data.sun = new Sun(sunPT, sunFrames);

            var tcloudmod = Content.Load<Model>("cloud");
            var cloudMod1 = Content.Load<Model>("cloud1");
            var cloudMod2 = Content.Load<Model>("cloud2");
            var cloudMod3 = Content.Load<Model>("cloud3");

            startScreen = Content.Load<Texture2D>("startscreen");
            startScale = Math.Max(graphics.GraphicsDevice.Viewport.Width / (float)startScreen.Width, graphics.GraphicsDevice.Viewport.Height / (float)startScreen.Height);

            startNoSelect = Content.Load<Texture2D>("start");
            startSelect = Content.Load<Texture2D>("startglow");
            exitNoSelect = Content.Load<Texture2D>("exit");
            exitSelect = Content.Load<Texture2D>("exitglow");

            Texture2D[] stars = new Texture2D[3];
            stars[0] = Content.Load<Texture2D>("stars");
            stars[1] = Content.Load<Texture2D>("stars2");
            stars[2] = Content.Load<Texture2D>("stars3");

            desWorldMat = Matrix.CreateRotationX(MathHelper.PiOver2);
            desWorldMat *= Matrix.CreateScale(20);

            float width = 1f;
            Data.proj = Matrix.CreatePerspective(width, width / graphics.GraphicsDevice.Viewport.AspectRatio, .5f, 100000f);
            renderHelper = new RenderHelper(renderEffect, graphics.GraphicsDevice, Data.proj);

            var windsound = Content.Load<SoundEffect>("wind");

            var phoneTs = new List<Texture2D>();
            for (int i = 0; i < 10; i++)
                phoneTs.Add(Content.Load<Texture2D>("gameboy" + (i + 1)));

            var phoneEs = new List<Texture2D>();
            for (int i = 0; i < 4; i++)
                phoneEs.Add(Content.Load<Texture2D>("enterpass" + (i + 1)));

            var phoneIs = new List<Texture2D>();
            for (int i = 0; i < 5; i++)
                phoneIs.Add(Content.Load<Texture2D>("input" + i));

            var noB = Content.Load<SoundEffect>("no");
            var yesB = Content.Load<SoundEffect>("yes");
            var searching = Content.Load<SoundEffect>("searching");
            var found = Content.Load<SoundEffect>("found");
            var success = Content.Load<SoundEffect>("sucess");
            Data.player = new Player(new Vector3(-1.9f * 8, -100, 0), playermodel, skate, windsound, phoneTs, phoneIs, phoneEs,yesB, noB,searching,found,success);

            Data.ramp = new Ramp(rampModel, 1.14014f, new Vector3(0, 0, 0));


            var satMod = Content.Load<Model>("satellite");
            Data.sattelite = new Sattelite(satMod, new Vector3(500,-500,200));

            var cloudPos = new Vector3(0, -400, 0);
            Random rand = new Random();
            for (int i = 0; i < 30; i++)
            {
                Model cloudMod;
                double r = rand.NextDouble();

                if (r < .3)
                    cloudMod = cloudMod1;
                else if (r < .6)
                    cloudMod = cloudMod2;
                else
                    cloudMod = cloudMod3;

                var xOffset = rand.Next(-1000, 5000);
                var yOffset = rand.Next(0, 2000);
                var zOffset = rand.Next(-1000, 5000);

                var pos = cloudPos + new Vector3(xOffset, -yOffset, zOffset);
                float speed = .5f * (float)rand.NextDouble();
                float scale = 100 + 500 * (float)rand.NextDouble() * yOffset / 4000;
                Data.clouds.Add(new Cloud(cloudMod, pos, speed, scale));
            }

            var starPos = new Vector3(0, -16000, 0);
            for (int i = 0; i < 10; i++)
            {
                var xOffset = rand.Next(-10000, 10000);
                var yOffset = rand.Next(0, 4000);
                var zOffset = rand.Next(-10000, 10000);

                var pos = starPos + new Vector3(xOffset, -yOffset, zOffset);
                Data.stars.Add(new PlaneTexture(stars[0], pos, 5f, graphics.GraphicsDevice));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case GameState.start:
                    updateStart();
                    break;
                case GameState.playing:
                    updateGame(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void updateGame(GameTime gameTime)
        {
            KeyboardManager.update();

            foreach (Cloud cloud in Data.clouds)
                cloud.update();

            float intensity = Math.Min(1, Math.Max(0, (-Data.player.position.Y - 500) / 3000));
           
            foreach (PlaneTexture pt in Data.stars)
            {

                
                pt.effect.LightingEnabled = true;

                pt.effect.DirectionalLight0.Direction =  new Vector3(.7f, 1, 0);
                pt.effect.DiffuseColor = Vector3.One;
                pt.effect.AmbientLightColor = new Vector3(1f, 1f, 1f);
                pt.effect.Alpha = intensity;
            }

            Data.sun.update(gameTime);

            Data.player.update(gameTime);

            if (Convert.ToString(Data.player.triedCode) == Data.sattelite.code)
            {
                markforwin = 5;
            }

            if (markforwin != 0)
            {
                markforwin--;

                if(markforwin == 1)
                {
                    Data.player.success.Play();
                }
            }

            if (Data.player.position.Y > 50)
            {
                this.Exit();
            }
        }

        private void updateStart()
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
                startSelected = true;
            else if (ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
                startSelected = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                MediaPlayer.Play(fly);
                MediaPlayer.Volume = .4f;
                gameState = GameState.playing;
            }
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.start:
                    drawStart();
                    break;
                case GameState.playing:
                    drawGame(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }

        private void drawGame(GameTime gameTime)
        {
            //Color clearColor = new Color(0.597f, 0.2f, 0.3f, 1f);
            Color clearColor = new Color(255f / 255, 103f / 255, 178f / 255);
            float c = Math.Max(0, 1 - Math.Max(0, -Data.player.position.Y - 500) / 1000);
            GraphicsDevice.Clear(clearColor * c);

            renderHelper.begin();
            Data.player.render(renderHelper);
            Data.ramp.render(renderHelper);
            renderHelper.renderModel(desModel, desWorldMat, Data.diffColor, desSpec, desAmb);

            foreach (Cloud cloud in Data.clouds)
                cloud.render(renderHelper);

            foreach (PlaneTexture pt in Data.stars)
                renderHelper.renderPlaneTex(pt);

            Data.sun.render(renderHelper);
            //Data.sattelite.render(renderHelper);

            ScoreBoard.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(Data.player.phoneCurT, new Vector2(graphics.GraphicsDevice.Viewport.Width * .8f, 
                graphics.GraphicsDevice.Viewport.Height - Data.player.phonePos), null, Color.White, 0, Vector2.Zero, Data.player.phoneScale, SpriteEffects.None, 1);
            spriteBatch.End();
        }

        private void drawStart()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(startScreen, Vector2.Zero, null, Color.White, 0, Vector2.Zero, startScale, SpriteEffects.None, 1);

            if (startSelected)
            {
                spriteBatch.Draw(startSelect, new Vector2(500, 500), Color.White);
                spriteBatch.Draw(exitNoSelect, new Vector2(500, 700), Color.White);
            }
            else
            {
                spriteBatch.Draw(startNoSelect, new Vector2(500, 500), Color.White);
                spriteBatch.Draw(exitSelect, new Vector2(500, 700), Color.White);
            }

            spriteBatch.End();
        }
    }
}
