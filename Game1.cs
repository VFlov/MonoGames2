using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGames2
{
    public class Game1 : Game
    {
        public void UpdateScreenAttributies()
        {
            Dx = (float)CurrentWidth / NominalWidth;
            Dy = (float)CurrentHeigth / NominalHeight;

            NominalHeightCounted = CurrentHeigth / Dx;
            NominalWidthCounted = CurrentWidth / Dx;

            int check = Math.Abs(CurrentHeigth - CurrentWidth / 16 * 9);
            if (check > 10)
                deltaY = (float)check / 2; // недостающее расстояние до 16:9 по п оси Y (в абсолютных координатах)
            deltaY_1 = -(CurrentWidth / 16 * 10 - CurrentWidth / 16 * 9) / 2f;

            YTopBorder = -deltaY / Dx; // координата точки в левом верхнем углу (в вируальных координатах)
            YBottomBorder = NominalHeight + (180); // координата точки в нижнем верхнем углу (в виртуальных координатах)
        }

        // коррекция координаты X
        public static float AbsoluteX(float x)
        {
            return x * Dx;
        }

        // коррекция координаты Y
        public static float AbsoluteY(float y)
        {
            return y * Dx + deltaY;
        }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D goodTexture; // наш спрайт
        Texture2D evilTexture; // второй спрайт
        Vector2 goodSpritePosition; // позиция нашего спрайта
        Vector2 evilSpritePosition; // позиция второго спрайта
        Point goodSpriteSize; // размер нашего спрайта
        Point evilSpriteSize; // размер второго спрайта
        float goodSpriteSpeed = 5f;
        float evilSpriteSpeed = 2f;

        Color color = Color.CornflowerBlue;
        Matrix projectionMatrix; // матрица проекции
        Matrix viewMatrix; // матрица вида
        Matrix worldMatrix; // мировая матрица

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            goodSpritePosition = Vector2.Zero;
            //  помещаем второй спрайт на середину по оси Х
            evilSpritePosition = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height/2);
        }

        protected override void Initialize()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 6), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Window.ClientBounds.Width /
                (float)Window.ClientBounds.Height,
                1, 100);

            worldMatrix = Matrix.Identity;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            goodTexture = Content.Load<Texture2D>("good");
            evilTexture = Content.Load<Texture2D>("evil");
            // Устанавливаем размеры спрайтов
            goodSpriteSize = new Point(goodTexture.Width, goodTexture.Height);
            evilSpriteSize = new Point(evilTexture.Width, evilTexture.Height);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // перемещаем второй спрайт
            evilSpritePosition.X += evilSpriteSpeed;
            if (evilSpritePosition.X > Window.ClientBounds.Width - evilTexture.Width || evilSpritePosition.X < 0)
                evilSpriteSpeed *= -1;

            // перемещаем наш спрайт клавиатурой
            if (keyboardState.IsKeyDown(Keys.Left))
                goodSpritePosition.X -= goodSpriteSpeed;
            if (keyboardState.IsKeyDown(Keys.Right))
                goodSpritePosition.X += goodSpriteSpeed;
            if (keyboardState.IsKeyDown(Keys.Up))
                goodSpritePosition.Y -= goodSpriteSpeed;
            if (keyboardState.IsKeyDown(Keys.Down))
                goodSpritePosition.Y += goodSpriteSpeed;

            // проверяем, не убежал ли наш спрайт с игрового поля
            if (goodSpritePosition.X < 0)
                goodSpritePosition.X = 0;
            if (goodSpritePosition.Y < 0)
                goodSpritePosition.Y = 0;
            if (goodSpritePosition.X > Window.ClientBounds.Width - goodSpriteSize.X)
                goodSpritePosition.X = Window.ClientBounds.Width - goodSpriteSize.X;
            if (goodSpritePosition.Y > Window.ClientBounds.Height - goodSpriteSize.Y)
                goodSpritePosition.Y = Window.ClientBounds.Height - goodSpriteSize.Y;

            if (Collide())
                color = Color.Red;
            else
                color = Color.CornflowerBlue;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(color);

            spriteBatch.Begin();

            spriteBatch.Draw(goodTexture, goodSpritePosition, Color.White);

            spriteBatch.Draw(evilTexture, evilSpritePosition, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected bool Collide()
        {
            Rectangle goodSpriteRect = new Rectangle((int)goodSpritePosition.X,
                (int)goodSpritePosition.Y, goodSpriteSize.X, goodSpriteSize.Y);
            Rectangle evilSpriteRect = new Rectangle((int)evilSpritePosition.X,
                (int)evilSpritePosition.Y, evilSpriteSize.X, evilSpriteSize.Y);

            return goodSpriteRect.Intersects(evilSpriteRect);
        }
    }
}