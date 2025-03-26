using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;
using static System.Windows.Forms.LinkLabel;

namespace KP_Stepanyan_PRI_121
{
    public partial class Form1 : Form
    {
        private float lionX = -2.0f;
        private float lionZ = 1.5f;
        float darkIndex = -0.5f;
        int alpha = 0;
        float beta = 0;

        private bool isInsideHouse = false;
        private bool isLightOn = false;

        public Form1()
        {
            InitializeComponent();
            Glut.glutInit();
            AnT.InitializeContexts();
            SetupScene();
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            AnT.KeyDown += new KeyEventHandler(OnKeyDown);
            RenderScene();

            this.Focus();
            this.KeyPreview = true; // Разрешаем форме обрабатывать события клавиш
            AnT.TabStop = true;

            AnT.MouseClick += (sender, e) =>
            {
                // Преобразуем координаты экрана в координаты сцены
                int viewportX = e.X;
                int viewportY = AnT.Height - e.Y; // Инвертируем Y

                // Получаем матрицы и viewport
                int[] viewport = new int[4];
                double[] modelview = new double[16];
                double[] projection = new double[16];

                Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
                Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelview);
                Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projection);

                // Координаты выключателя в 3D пространстве
                double[] switchPos = { 6.5f, -3.0f, -4.98f };

                // Проецируем 3D координаты в 2D экранные
                double[] screenPos = new double[3];
                Glu.gluProject(switchPos[0], switchPos[1], switchPos[2],
                              modelview, projection, viewport,
                              out screenPos[0], out screenPos[1], out screenPos[2]);

                // Проверяем попадание в область выключателя
                float switchSize = 50f; // Размер области клика в пикселях
                if (Math.Abs(e.X - screenPos[0]) < switchSize &&
                    Math.Abs(viewportY - screenPos[1]) < switchSize)
                {
                    isLightOn = !isLightOn;
                    RenderScene();
                }
            };
            


        }

        private void SetupScene()
        {
            // Цвет неба
            Gl.glClearColor(0.5f, 0.8f, 1.0f, 1.0f); // Цвет фона
            
        }

        private void RenderScene()
        {
            if (isInsideHouse)
            {
                RenderHouseInterior(); // Рисуем интерьер дома
            }
            else
            {
                // Оригинальная сцена (вне дома)
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                // Устанавливаем матрицу проекции
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                Glu.gluPerspective(45.0, AnT.Width / (double)AnT.Height, 0.1, 100.0);

                // Устанавливаем модельно-видовую матрицу
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();
                Glu.gluLookAt(0, 5, 15, 0, 0, 0, 0, 1, 0);

                // Зеленая равнина
                Gl.glColor3f(0.0f, 0.4f, 0.0f);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(-10.0f, 0.0f, -10.0f);
                Gl.glVertex3f(-10.0f, 0.0f, 10.0f);
                Gl.glVertex3f(10.0f, 0.0f, 10.0f);
                Gl.glVertex3f(10.0f, 0.0f, -10.0f);
                Gl.glEnd();

                // Тропинка
                Gl.glColor3f(0.8f, 0.6f, 0.0f);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(-1.0f, 0.01f, -10.0f);
                Gl.glVertex3f(-1.0f, 0.01f, 10.0f);
                Gl.glVertex3f(1.0f, 0.01f, 10.0f);
                Gl.glVertex3f(1.0f, 0.01f, -10.0f);
                Gl.glEnd();

                // Деревья
                DrawTree(-5.0f, 0.0f, -5.0f);
                DrawTree(3.0f, 0.0f, -3.0f);
                DrawTree(-8.0f, 0.0f, 6.0f);

                // Домик
                DrawHouse(3.0f, 0.0f, 0.0f);

                // Лев
                DrawLion(lionX, -1.9f, lionZ);

                // Журавль
                DrawCrane(5.0f, -2.0f, -1.0f);

                DrawFractalCloud(-5.0f, 5.0f, -8.0f, 1.5f, 3);
                DrawFractalCloud(2.0f, 6.0f, -10.0f, 2.0f, 2);
                DrawFractalCloud(5.0f, 4.5f, -5.0f, 1.2f, 3);

               

                AnT.Refresh();
            }
        }

        private void SetupLighting()
        {
            if (isLightOn)
            {
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_LIGHT0);
                float[] lightPos = { 0.0f, 5.0f, 10.0f, 1.0f };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);
            }
            else
            {
                Gl.glDisable(Gl.GL_LIGHTING);
            }
        }

        //Лев
        private void DrawLion(float x, float y, float z)
        {
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z);

            // Тело льва (сфера)
            Gl.glColor3f(0.4f, 0.3f, 0.1f);
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 0.0f, 0.0f);
            Glu.GLUquadric bodyQuadric = Glu.gluNewQuadric();
            Glu.gluSphere(bodyQuadric, 1.6f, 16, 16); 
            Gl.glPopMatrix();

            // Голова льва (сфера)
            Gl.glColor3f(0.2f, 0.2f, 0.1f);
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 1.5f, 1.5f); 
            Glu.gluSphere(bodyQuadric, 1.7f, 16, 16); 
            Gl.glPopMatrix();


            // Уши
            //Gl.glColor3f(0.5f, 0.3f, 0.1f);
            //Gl.glPushMatrix();
            //Gl.glTranslatef(-0.8f, 2.0f, 1.5f); // Левое ухо
            //Glu.gluSphere(bodyQuadric, 0.4f, 8, 8); 
            //Gl.glPopMatrix();

            //Gl.glPushMatrix();
            //Gl.glTranslatef(0.8f, 2.0f, 1.5f); // Правое ухо
            //Glu.gluSphere(bodyQuadric, 0.4f, 8, 8); 
            //Gl.glPopMatrix();


            // Хвост льва (цилиндр)
            Gl.glColor3f(0.5f, 0.3f, 0.1f);
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, -1.5f, -1.0f); 
            Gl.glRotatef(90, 1.0f, 0.0f, 0.0f); 
            Glu.gluCylinder(bodyQuadric, 0.3f, 0.3f, 2.0f, 8, 8); 
            Gl.glPopMatrix();

            Glu.gluDeleteQuadric(bodyQuadric); 

            Gl.glPopMatrix(); 
        }

        //Журавль
        private void DrawCrane(float x, float y, float z)
        {
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z); 

            // Тело журавля (сфера)
            Gl.glColor3f(0.8f, 0.8f, 0.8f); 
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 1.0f, 0.0f); 
            Glu.GLUquadric bodyQuadric = Glu.gluNewQuadric();
            Glu.gluSphere(bodyQuadric, 0.5f, 16, 16); 
            Gl.glPopMatrix();

            // Голова журавля (сфера)
            Gl.glColor3f(0.8f, 0.8f, 0.8f); 
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 1.5f, 0.6f); 
            Glu.gluSphere(bodyQuadric, 0.3f, 16, 16); 
            Gl.glPopMatrix();

            // Клюв журавля (цилиндр)
            Gl.glColor3f(1.0f, 0.6f, 0.0f); 
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 1.5f, 1.0f); 
            Gl.glRotatef(45.0f, 0.0f, 0.0f, 1.0f); 
            Glu.gluCylinder(bodyQuadric, 0.05f, 0.0f, 0.3f, 8, 8); 
            Gl.glPopMatrix();

            // Ноги журавля (цилиндры)
            Gl.glColor3f(0.8f, 0.4f, 0.1f); 
            Gl.glPushMatrix();
            Gl.glTranslatef(-0.2f, 0.0f, 0.0f); 
            Gl.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            Glu.gluCylinder(bodyQuadric, 0.05f, 0.05f, 1.0f, 8, 8); 
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(0.2f, 0.0f, 0.0f); 
            Gl.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            Glu.gluCylinder(bodyQuadric, 0.05f, 0.05f, 1.0f, 8, 8); 
            Gl.glPopMatrix();

            // Крылья журавля (плоские прямоугольники)
            Gl.glColor3f(0.8f, 0.8f, 0.8f); 
            Gl.glPushMatrix();
            Gl.glTranslatef(0.5f, 1.0f, 0.0f);
            Gl.glRotatef(45.0f, 0.0f, 1.0f, 0.0f); 
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-0.1f, 0.0f, -0.3f);
            Gl.glVertex3f(0.1f, 0.0f, -0.3f);
            Gl.glVertex3f(0.1f, 0.0f, 0.3f);
            Gl.glVertex3f(-0.1f, 0.0f, 0.3f);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(-0.5f, 1.0f, 0.0f); 
            Gl.glRotatef(-45.0f, 0.0f, 1.0f, 0.0f); 
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-0.1f, 0.0f, -0.3f);
            Gl.glVertex3f(0.1f, 0.0f, -0.3f);
            Gl.glVertex3f(0.1f, 0.0f, 0.3f);
            Gl.glVertex3f(-0.1f, 0.0f, 0.3f);
            Gl.glEnd();
            Gl.glPopMatrix();

            Glu.gluDeleteQuadric(bodyQuadric);

            Gl.glPopMatrix();
        }


        //Рисование дерева
        private void DrawTree(float x, float y, float z)
        {
            // Рисуем ствол (цилиндр)
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z);
            Gl.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            Gl.glColor3f(0.55f, 0.27f, 0.07f);
            Glu.GLUquadric quadric = Glu.gluNewQuadric();
            Glu.gluCylinder(quadric, 0.2, 0.2, 2.0, 16, 16);
            Gl.glPopMatrix();

            // Рисуем крону (конус)
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y + 2.0f, z);
            Gl.glColor3f(0.2f, 0.6f, 0.2f);
            Gl.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            Glu.gluCylinder(quadric, 1.0, 0.0, 2.0, 16, 16);
            Gl.glPopMatrix();

            Glu.gluDeleteQuadric(quadric);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            const float moveStep = 0.5f;
            float newLionX = lionX;
            float newLionZ = lionZ;

            switch (e.KeyCode)
            {
                case Keys.W:
                    newLionZ -= moveStep;
                    break;
                case Keys.S:
                    newLionZ += moveStep;
                    break;
                case Keys.A:
                    newLionX -= moveStep;
                    break;
                case Keys.D:
                    newLionX += moveStep;
                    break;
                case Keys.T:
                    if (isLightOn)
                    {
                        darkIndex = -0.5f;
                        isLightOn = false;
                    }
                    else
                    {
                        darkIndex = 0f;
                        isLightOn = true;
                    }
                    break;
            }

            // Проверяем столкновения
            if (!CheckCollision(newLionX, newLionZ))
            {
                lionX = newLionX;
                lionZ = newLionZ;
            }

            // Перерисовываем сцену с новыми координатами льва
            RenderScene();
        }


        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            RenderScene();
        }

        private bool CheckCollision(float newX, float newZ)
        {
            // Границы дома
            float houseX = 3.0f;
            float houseZ = 0.0f;
            float houseWidth = 4.0f; // Ширина дома
            float houseDepth = 4.0f; // Глубина дома

            // Проверка столкновения с домом
            if (newX >= houseX - houseWidth / 2 && newX <= houseX + houseWidth / 2 &&
                newZ >= houseZ - houseDepth / 2 && newZ <= houseZ + houseDepth / 2)
            {
                return true; // Столкновение с домом
            }

            return false; // Нет столкновений
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Добавление домика
        private void DrawHouse(float x, float y, float z)
        {
            // Основание домика (внешние стены)
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z);
            Gl.glColor3f(0.8f, 0.5f, 0.2f); // Цвет стен

            // Внешние стены
            DrawCube(-2.0f, 0.0f, -2.0f, 4.0f, 2.0f, 4.0f); // Внешний куб

            // Внутренние стены (пустое пространство)
            Gl.glColor3f(0.7f, 0.4f, 0.1f); // Цвет внутренних стен
            DrawCube(-1.8f, 0.1f, -1.8f, 3.6f, 1.8f, 3.6f); // Внутренний куб (меньше внешнего)

            // Крыша
            Gl.glColor3f(0.5f, 0.2f, 0.1f); // Цвет крыши
            Gl.glBegin(Gl.GL_TRIANGLES);
            // Передняя часть крыши
            Gl.glVertex3f(-2.0f, 2.0f, 2.0f);
            Gl.glVertex3f(2.0f, 2.0f, 2.0f);
            Gl.glVertex3f(0.0f, 3.0f, 2.0f);

            // Задняя часть крыши
            Gl.glVertex3f(-2.0f, 2.0f, -2.0f);
            Gl.glVertex3f(2.0f, 2.0f, -2.0f);
            Gl.glVertex3f(0.0f, 3.0f, -2.0f);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            // Левая часть крыши
            Gl.glVertex3f(-2.0f, 2.0f, -2.0f);
            Gl.glVertex3f(-2.0f, 2.0f, 2.0f);
            Gl.glVertex3f(0.0f, 3.0f, 2.0f);
            Gl.glVertex3f(0.0f, 3.0f, -2.0f);

            // Правая часть крыши
            Gl.glVertex3f(2.0f, 2.0f, -2.0f);
            Gl.glVertex3f(2.0f, 2.0f, 2.0f);
            Gl.glVertex3f(0.0f, 3.0f, 2.0f);
            Gl.glVertex3f(0.0f, 3.0f, -2.0f);
            Gl.glEnd();

            ////Циферблат
            //Gl.glPushMatrix();
            //Gl.glTranslated(90, 199, 55);
            //Gl.glColor3f(0.72f + darkIndex, 0.79f + darkIndex, 0.25f + darkIndex);
            ////Gl.glDisable(Gl.GL_DEPTH_TEST);
            //Gl.glBegin(Gl.GL_QUADS);
            //Gl.glVertex3d(0, 0, 0);
            //Gl.glVertex3d(0, 0, 15);
            //Gl.glVertex3d(15, 0, 15);
            //Gl.glVertex3d(15, 0, 0);
            //Gl.glEnd();
            ////Gl.glEnable(Gl.GL_DEPTH_TEST);
            //Gl.glPopMatrix();

            ////Стрелки
            //Gl.glPushMatrix();
            //Gl.glTranslated(97.5, 197, 62.5);
            //Gl.glColor3f(0, 0, 0);
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            //Gl.glLineWidth(1f);
            //Gl.glRotatef(alpha, 0, 1, 0);
            //Gl.glBegin(Gl.GL_LINES);
            //Gl.glVertex3d(0, 0, 0);
            //Gl.glVertex3d(0, 0, 3.5);
            //Gl.glEnd();
            //alpha += 12;
            //Gl.glEnable(Gl.GL_DEPTH_TEST);
            //Gl.glPopMatrix();

            //Gl.glPushMatrix();
            //Gl.glTranslated(97.5, 197, 62.5);
            //Gl.glColor3f(0, 0, 0);
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            //Gl.glLineWidth(2f);
            //Gl.glRotatef(beta, 0, 1, 0);
            //Gl.glBegin(Gl.GL_LINES);
            //Gl.glVertex3d(0, 0, 0);
            //Gl.glVertex3d(0, 0, 2);
            //Gl.glEnd();
            //beta += 1;
            //Gl.glEnable(Gl.GL_DEPTH_TEST);
            //Gl.glPopMatrix();

            // Дверь
            Gl.glColor3f(0.1f, 0.1f, 0.3f); // Черный цвет двери
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-0.5f, 0.0f, 2.01f);
            Gl.glVertex3f(0.5f, 0.0f, 2.01f);
            Gl.glVertex3f(0.5f, 1.0f, 2.01f);
            Gl.glVertex3f(-0.5f, 1.0f, 2.01f);
            Gl.glEnd();

            Gl.glPopMatrix();
        }

        private void DrawCube(float x, float y, float z, float width, float height, float depth)
        {
            Gl.glBegin(Gl.GL_QUADS);

            // Передняя грань
            Gl.glVertex3f(x, y, z);
            Gl.glVertex3f(x + width, y, z);
            Gl.glVertex3f(x + width, y + height, z);
            Gl.glVertex3f(x, y + height, z);

            // Задняя грань
            Gl.glVertex3f(x, y, z + depth);
            Gl.glVertex3f(x + width, y, z + depth);
            Gl.glVertex3f(x + width, y + height, z + depth);
            Gl.glVertex3f(x, y + height, z + depth);

            // Левая грань
            Gl.glVertex3f(x, y, z);
            Gl.glVertex3f(x, y, z + depth);
            Gl.glVertex3f(x, y + height, z + depth);
            Gl.glVertex3f(x, y + height, z);

            // Правая грань
            Gl.glVertex3f(x + width, y, z);
            Gl.glVertex3f(x + width, y, z + depth);
            Gl.glVertex3f(x + width, y + height, z + depth);
            Gl.glVertex3f(x + width, y + height, z);

            // Верхняя грань
            Gl.glVertex3f(x, y + height, z);
            Gl.glVertex3f(x + width, y + height, z);
            Gl.glVertex3f(x + width, y + height, z + depth);
            Gl.glVertex3f(x, y + height, z + depth);

            // Нижняя грань
            Gl.glVertex3f(x, y, z);
            Gl.glVertex3f(x + width, y, z);
            Gl.glVertex3f(x + width, y, z + depth);
            Gl.glVertex3f(x, y, z + depth);

            Gl.glEnd();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isInsideHouse = true; // Переключаемся на сцену внутри дома
            RenderScene();
        }

        private void DrawFractalCloud(float centerX, float centerY, float centerZ, float size, int detail)
        {
            Gl.glColor3f(1.0f, 1.0f, 1.0f); // Белый цвет для облака
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);

            // Центральная точка облака
            Gl.glVertex3f(centerX, centerY, centerZ);

            // Генерируем точки по окружности с фрактальными вариациями
            Random rand = new Random();
            for (int i = 0; i <= 360; i += 10)
            {
                float angle = (float)(i * Math.PI / 180);
                float radius = size * (0.7f + (float)rand.NextDouble() * 0.3f);

                // Добавляем фрактальную детализацию
                for (int d = 0; d < detail; d++)
                {
                    radius *= 0.9f + (float)rand.NextDouble() * 0.2f;
                }

                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle) * 0.6f; // Делаем облако немного плоским

                Gl.glVertex3f(x, y, centerZ);
            }

            Gl.glEnd();
        }

        private void RenderHouseInterior()
        {

            SetupLighting();

            // Очистка экрана и буфера глубины
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            // Устанавливаем матрицу проекции
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0, AnT.Width / (double)AnT.Height, 0.1, 100.0);

            // Устанавливаем модельно-видовую матрицу
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 5, 15, 0, 0, 0, 0, 1, 0);

            // Потолок
            Gl.glPushMatrix();
            Gl.glColor3f(0.4f, 0.2f, 0.1f);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(200, -10, 100);
            Gl.glVertex3d(-100, -10, 100);
            Gl.glVertex3d(-100, 200, 100);
            Gl.glVertex3d(200, 200, 100);
            Gl.glEnd();

            // Пол
            Gl.glPushMatrix();
            Gl.glColor3f(0.4f, 0.2f, 0.1f);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(200, -10, 5);
            Gl.glVertex3d(-100, -10, 5);
            Gl.glVertex3d(-100, 200, 5);
            Gl.glVertex3d(200, 200, 5);
            Gl.glEnd();

            // Передняя стена (на весь экран)
            Gl.glColor3f(0.6f, 0.3f, 0.1f); // Коричневый
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-10.0f, -10.0f, -5.0f);
            Gl.glVertex3f(10.0f, -10.0f, -5.0f);
            Gl.glVertex3f(10.0f, 10.0f, -5.0f);
            Gl.glVertex3f(-10.0f, 10.0f, -5.0f);
            Gl.glEnd();

            // Окно (голубое)
            Gl.glColor3f(0.2f, 0.6f, 0.8f);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-6.0f, -5.0f, -4.99f);
            Gl.glVertex3f(6.0f, -5.0f, -4.99f);
            Gl.glVertex3f(6.0f, 1.0f, -4.99f);
            Gl.glVertex3f(-6.0f, 1.0f, -4.99f);
            Gl.glEnd();

            float switchX = 6.5f; // Правее окна
            float switchY = -3.0f; // На уровне середины окна
            float switchZ = -4.98f; // Почти на той же глубине что и окно
            float switchWidth = 0.5f;
            float switchHeight = 0.8f;

            if (isLightOn)
            {
                // Выключатель в положении "вкл" (горизонтально)
                Gl.glColor3f(0.9f, 0.9f, 0.6f); // Темно-серый
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(switchX, switchY, switchZ);
                Gl.glVertex3f(switchX + switchWidth, switchY, switchZ);
                Gl.glVertex3f(switchX + switchWidth, switchY + switchHeight / 3, switchZ);
                Gl.glVertex3f(switchX, switchY + switchHeight / 3, switchZ);
                Gl.glEnd();
            }
            else
            {
                // Выключатель в положении "выкл" (вертикально)
                Gl.glColor3f(0.4f, 0.4f, 0.4f); // Темно-серый
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(switchX, switchY, switchZ);
                Gl.glVertex3f(switchX + switchHeight / 3, switchY, switchZ);
                Gl.glVertex3f(switchX + switchHeight / 3, switchY + switchWidth, switchZ);
                Gl.glVertex3f(switchX, switchY + switchWidth, switchZ);
                Gl.glEnd();
            }

            // Рамка выключателя
            Gl.glColor3f(0.1f, 0.1f, 0.1f); // Черный
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(switchX - 0.1f, switchY - 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX + switchWidth + 0.1f, switchY - 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX + switchWidth + 0.1f, switchY + switchHeight + 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX - 0.1f, switchY + switchHeight + 0.1f, switchZ - 0.01f);
            Gl.glEnd();
            Gl.glLineWidth(1.0f);

            // Рисуем "картину" с фрактальной линией (кривая дракона)
            float[] yellowColor = new float[] { 1.0f, 1.0f, 0.0f }; // R, G, B
            DrawFractalDragonCurve(2.0f, 3.5f, -4.98f, 2.5f, 12, yellowColor);

            // Второй фрактал - зеленый
            float[] greenColor = new float[] { 0.0f, 1.0f, 0.0f }; // R, G, B
            DrawFractalDragonCurve(-5.0f, 3.5f, -4.98f, 2.5f, 9, greenColor);


            DrawLion(0.0f, -4.9f, 1.0f); // Рисуем льва внутри дома
            DrawCrane(3.0f, -2.9f, 5.0f); // Рисуем журавля (парикхмахера)
            DrawTable(-5.0f, -6.0f, 1.5f); // Рисуем стол

            AnT.Refresh();
        }

        // Метод для рисования фрактальной кривой дракона
        private void DrawFractalDragonCurve(float x, float y, float z, float size, int iterations, float[] color)
        {
            List<PointF> points = new List<PointF>();
            points.Add(new PointF(x, y));
            points.Add(new PointF(x + size, y));

            for (int i = 0; i < iterations; i++)
            {
                List<PointF> newPoints = new List<PointF>();
                newPoints.Add(points[0]);

                for (int j = 1; j < points.Count; j++)
                {
                    PointF midPoint = new PointF(
                        (points[j - 1].X + points[j].X) / 2 + (points[j - 1].Y - points[j].Y) / 2,
                        (points[j - 1].Y + points[j].Y) / 2 - (points[j - 1].X - points[j].X) / 2
                    );

                    newPoints.Add(midPoint);
                    newPoints.Add(points[j]);
                }

                points = newPoints;
            }

            // Рисуем полученную кривую с заданным цветом
            Gl.glColor3fv(color); // Используем переданный цвет
            Gl.glBegin(Gl.GL_LINE_STRIP);
            foreach (PointF p in points)
            {
                Gl.glVertex3f(p.X, p.Y, z);
            }
            Gl.glEnd();

            // Увеличенная рамка для картины
            Gl.glColor3f(0.0f, 0.0f, 0.0f); // Черный цвет
            Gl.glLineWidth(3.0f); // Более светлая коричневая рамка
            float frameWidth = size * 0.7f; // Ширина рамки (20% от размера)
            float frameHeight = size * 1.0f; // Высота рамки (30% от размера)

            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(x - frameWidth, y - frameHeight, z - 0.01f);
            Gl.glVertex3f(x + size + frameWidth, y - frameHeight, z - 0.01f);
            Gl.glVertex3f(x + size + frameWidth, y + frameHeight, z - 0.01f);
            Gl.glVertex3f(x - frameWidth, y + frameHeight, z - 0.01f);
            Gl.glEnd();
        }

        

        private void DrawTable(float x, float y, float z)
        {
            Gl.glPushMatrix();
            Gl.glTranslatef(x, y, z);

            // Цвет стола (серый)
            Gl.glColor3f(0.5f, 0.5f, 0.5f);

            // Столешница (верхняя часть стола)
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-2.0f, 1.5f, -1.0f); 
            Gl.glVertex3f(2.0f, 1.5f, -1.0f);  
            Gl.glVertex3f(2.0f, 1.5f, 1.0f);   
            Gl.glVertex3f(-2.0f, 1.5f, 1.0f);  
            Gl.glEnd();

            // Ножки стола (4 вертикальных прямоугольника)
            Gl.glBegin(Gl.GL_QUADS);

            // Левая передняя ножка
            Gl.glVertex3f(-1.8f, 0.0f, -0.8f);
            Gl.glVertex3f(-1.5f, 0.0f, -0.8f);
            Gl.glVertex3f(-1.5f, 1.5f, -0.8f);
            Gl.glVertex3f(-1.8f, 1.5f, -0.8f);

            // Правая передняя ножка
            Gl.glVertex3f(1.5f, 0.0f, -0.8f);
            Gl.glVertex3f(1.8f, 0.0f, -0.8f);
            Gl.glVertex3f(1.8f, 1.5f, -0.8f);
            Gl.glVertex3f(1.5f, 1.5f, -0.8f);

            // Левая задняя ножка
            Gl.glVertex3f(-1.8f, 0.0f, 0.8f);
            Gl.glVertex3f(-1.5f, 0.0f, 0.8f);
            Gl.glVertex3f(-1.5f, 1.5f, 0.8f);
            Gl.glVertex3f(-1.8f, 1.5f, 0.8f);

            // Правая задняя ножка
            Gl.glVertex3f(1.5f, 0.0f, 0.8f);
            Gl.glVertex3f(1.8f, 0.0f, 0.8f);
            Gl.glVertex3f(1.8f, 1.5f, 0.8f);
            Gl.glVertex3f(1.5f, 1.5f, 0.8f);

            Gl.glEnd();

            Gl.glPopMatrix();
        }



        // Выйти из парикмахерской
        private void button3_Click(object sender, EventArgs e)
        {
            isInsideHouse = false; // Переключаемся на сцену вне дома
            RenderScene();
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
