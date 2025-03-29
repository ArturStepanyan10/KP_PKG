using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace KP_Stepanyan_PRI_121
{
    public partial class Form1 : Form
    {
        private float lionX = -2.0f;
        private float lionZ = 1.5f;

        private bool isInsideHouse = false;
        private bool isLightOn = false;

        private float cloudOffsetX = 0.0f;
        private readonly float cloudSpeed = 0.05f;

        private float craneX = 5.0f;
        private float craneY = -2.0f;
        private float craneZ = -1.0f;
        private float wingAngle = 0.0f;
        private bool wingDirectionUp = true;

        private const float MIN_LION_X = -8.0f; // Минимальная X-координата (влево)
        private const float MAX_LION_X = 6.0f;  // Максимальная X-координата (вправо)
        private const float MIN_LION_Z = -8.0f; // Минимальная Z-координата (назад)
        private const float MAX_LION_Z = 8.0f;

        private const float MIN_CRANE_Y = -3.0f; // Минимальная высота (ближе к земле)
        private const float MAX_CRANE_Y = 3.0f;

        private float lionHeadSize = 1.7f;     // Стандартный размер головы
        private bool hasEars = false;          // Флаг наличия ушек
        private bool isSmallHeadMode = false;

        private bool isSharpnessEffectActive = false;

        private float vaseRotationAngle = 0.0f; // Угол вращения вазы
        private float vaseRotationSpeed = 1.0f;

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

            RenderTimer.Start();
        }

        private void SetupScene()
        {
            Gl.glClearColor(0.5f, 0.8f, 1.0f, 1.0f); // Цвет неба
        }

        private void RenderScene()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

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
                if (isSharpnessEffectActive)
                {
                    DrawEnhancedHouseAndTrees();
                }
                else
                {
                    DrawHouse(3.0f, 0.0f, 0.0f);
                    DrawTree(-5.0f, 0.0f, -5.0f);
                    DrawTree(-8.0f, 0.0f, 1.0f);
                    DrawTree(-5.0f, 0.0f, 6.0f);
                }

                // Лев
                DrawLion(lionX, -1.9f, lionZ);

                // Журавль
                DrawCrane(craneX, craneY, craneZ);

                // Облака
                DrawFractalCloud(-5.0f + cloudOffsetX, 5.0f, -8.0f, 1.5f, 3);
                DrawFractalCloud(2.0f + cloudOffsetX * 0.8f, 6.0f, -10.0f, 2.0f, 2); 
                DrawFractalCloud(5.0f + cloudOffsetX * 1.2f, 4.5f, -5.0f, 1.2f, 3);
                
                AnT.Refresh();
            }
        }

        private void DrawEnhancedHouseAndTrees()
        {
            DrawHouse(3.0f, 0.0f, 0.0f);
            DrawTree(-5.0f, 0.0f, -5.0f);
            DrawTree(-8.0f, 0.0f, 1.0f);
            DrawTree(-5.0f, 0.0f, 6.0f);

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_LINE);
            Gl.glPolygonOffset(-1.0f, -1.0f);
            Gl.glLineWidth(1.5f);
            Gl.glColor3f(0.2f, 0.2f, 0.2f);
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);

            DrawHouse(3.0f, 0.0f, 0.0f);
            DrawTree(-5.0f, 0.0f, -5.0f);
            DrawTree(-8.0f, 0.0f, 1.0f);
            DrawTree(-5.0f, 0.0f, 6.0f);

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE);
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

            // Тело льва
            Gl.glColor3f(0.4f, 0.3f, 0.1f);
            Glu.GLUquadric body = Glu.gluNewQuadric();
            Glu.gluSphere(body, 1.6f, 16, 16);
            Glu.gluDeleteQuadric(body);

            // Голова льва
            Gl.glColor3f(0.2f, 0.2f, 0.1f);
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 1.5f, 1.5f);
            Glu.GLUquadric head = Glu.gluNewQuadric();
            Glu.gluSphere(head, lionHeadSize, 16, 16);
            
            // Рисуем ушки если включены
            if (hasEars)
            {
                Gl.glColor3f(0.2f, 0.1f, 0.05f); // Темный цвет ушек

                // Левое ухо
                Gl.glPushMatrix();
                Gl.glTranslatef(-0.5f, 0.8f, 0.3f);
                Glu.gluSphere(head, 0.3f, 8, 8);

                // Правое ухо
                Gl.glPushMatrix();
                Gl.glTranslatef(1.1f, 0.3f, 0.3f);
                Glu.gluSphere(head, 0.3f, 8, 8);          
            }
            
            Glu.gluDeleteQuadric(head);

            // Хвост льва
            Gl.glColor3f(0.5f, 0.3f, 0.1f);    
            Gl.glTranslatef(0.0f, -2.5f, -1.0f);
            Gl.glRotatef(90, 1.0f, 0.0f, 0.5f);
            Glu.GLUquadric tail = Glu.gluNewQuadric();
            Glu.gluCylinder(tail, 0.3f, 0.3f, 2.0f, 6, 6);
            Glu.gluDeleteQuadric(tail);
            Gl.glPopMatrix();

            Gl.glPopMatrix();
        }

        //Журавль
        private void DrawCrane(float x, float y, float z)
        {
            bool isOnGround = y <= MIN_CRANE_Y + 0.1f;

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

            if (isOnGround)
            {
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
            }

            // Крылья журавля (плоские прямоугольники)
            Gl.glColor3f(0.8f, 0.8f, 0.8f);

            // Левое крыло
            Gl.glPushMatrix();
            Gl.glTranslatef(-0.5f, 1.0f, 0.0f);
            Gl.glRotatef(wingAngle, 1.0f, 0.0f, 0.0f);
            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glVertex3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(-0.8f, 0.0f, -0.5f);
            Gl.glVertex3f(-0.8f, 0.0f, 0.5f);
            Gl.glEnd();
            Gl.glPopMatrix();

            // Правое крыло
            Gl.glPushMatrix();
            Gl.glTranslatef(0.5f, 1.0f, 0.0f);
            Gl.glRotatef(-wingAngle, 1.0f, 0.0f, 0.0f);
            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glVertex3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(0.8f, 0.0f, -0.5f);
            Gl.glVertex3f(0.8f, 0.0f, 0.5f);
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
            float newCraneY = craneY;

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
                case Keys.G:
                    isLightOn = !isLightOn;
                    break;
                case Keys.NumPad8:
                    newCraneY += moveStep;
                    break;
                case Keys.NumPad2:
                    newCraneY -= moveStep;
                    break;
            }

            newLionX = Math.Max(MIN_LION_X, Math.Min(MAX_LION_X, newLionX));
            newLionZ = Math.Max(MIN_LION_Z, Math.Min(MAX_LION_Z, newLionZ));

            // Проверяем столкновения
            if (!CheckCollision(newLionX, newLionZ))
            {
                lionX = newLionX;
                lionZ = newLionZ;
            }

            craneY = Math.Max(MIN_CRANE_Y, Math.Min(MAX_CRANE_Y, newCraneY));

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
            float houseWidth = 4.0f; 
            float houseDepth = 4.0f; 

            // Проверка столкновения с домом
            if (newX >= houseX - houseWidth / 2 && newX <= houseX + houseWidth / 2 &&
                newZ >= houseZ - houseDepth / 2 && newZ <= houseZ + houseDepth / 2)
            {
                return true; 
            }
            return false; 
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
            Gl.glColor3f(0.5f, 0.2f, 0.1f);
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

            // Дверь
            Gl.glColor3f(0.1f, 0.1f, 0.3f);
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
            isInsideHouse = true; 
            RenderScene();
        }

        private void DrawFractalCloud(float centerX, float centerY, float centerZ, float size, int detail)
        {
            Random rand = new Random(GetCloudSeed(centerX, centerY, centerZ));

            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3f(centerX, centerY, centerZ);

            for (int i = 0; i <= 360; i += 10)
            {
                float angle = (float)(i * Math.PI / 180);
                float radius = size * (0.7f + (float)rand.NextDouble() * 0.3f);

                for (int d = 0; d < detail; d++)
                {
                    radius *= 0.9f + (float)rand.NextDouble() * 0.2f;
                }

                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle) * 0.6f;
                Gl.glVertex3f(x, y, centerZ);
            }
            Gl.glEnd();
        }

        private int GetCloudSeed(float x, float y, float z)
        {
            return (int)(x * 100 + y * 10 + z);
        }

        private void DrawRevolvedVase()
        {
            // Точки профиля вазы (X - радиус, Y - высота)
            List<PointF> profilePoints = new List<PointF>
            {
                new PointF(0.0f, 0.0f),   // Основание
                new PointF(0.5f, 0.5f),   // Начало изгиба
                new PointF(1.2f, 1.5f),   // Самая широкая часть
                new PointF(0.8f, 2.5f),   // Сужение
                new PointF(0.4f, 3.0f),   // Горлышко
                new PointF(0.0f, 3.5f)    // Верх
            };

            // Интерполяция для гладкости
            List<PointF> interpolatedPoints = InterpolateLagrange(profilePoints, 30);
            int numSlices = 36; // Количество сегментов по окружности

            // Цвет вазы (фиолетовый)
            Gl.glColor3f(0.8f, 0.6f, 0.9f);

            // Рисуем тело вращения
            for (int i = 0; i < numSlices; i++)
            {
                float angle1 = (float)(i * 2.0 * Math.PI / numSlices);
                float angle2 = (float)((i + 1) * 2.0 * Math.PI / numSlices);

                Gl.glBegin(Gl.GL_QUAD_STRIP);
                for (int j = 0; j < interpolatedPoints.Count; j++)
                {
                    float x = interpolatedPoints[j].X;
                    float y = interpolatedPoints[j].Y;

                    // Точка 1
                    float x1 = x * (float)Math.Cos(angle1);
                    float z1 = x * (float)Math.Sin(angle1);
                    Gl.glVertex3f(x1, y, z1);

                    // Точка 2
                    float x2 = x * (float)Math.Cos(angle2);
                    float z2 = x * (float)Math.Sin(angle2);
                    Gl.glVertex3f(x2, y, z2);
                }
                Gl.glEnd();
            }
        }

        private void RenderHouseInterior()
        {
            SetupLighting();
            
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
       
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0, AnT.Width / (double)AnT.Height, 0.1, 100.0);

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

            // Зеркало (голубое)
            Gl.glColor3f(0.2f, 0.6f, 0.8f);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-6.0f, -5.0f, -4.99f);
            Gl.glVertex3f(6.0f, -5.0f, -4.99f);
            Gl.glVertex3f(6.0f, 1.0f, -4.99f);
            Gl.glVertex3f(-6.0f, 1.0f, -4.99f);
            Gl.glEnd(); 
            
            float switchX = 6.5f; 
            float switchY = -3.0f; 
            float switchZ = -4.98f; 
            float switchWidth = 0.5f;
            float switchHeight = 0.8f;

            if (isLightOn)
            {
                // Выключатель в положении "вкл" (горизонтально)
                Gl.glColor3f(0.9f, 0.9f, 0.6f); 
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
                Gl.glColor3f(0.4f, 0.4f, 0.4f);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex3f(switchX, switchY, switchZ);
                Gl.glVertex3f(switchX + switchHeight / 3, switchY, switchZ);
                Gl.glVertex3f(switchX + switchHeight / 3, switchY + switchWidth, switchZ);
                Gl.glVertex3f(switchX, switchY + switchWidth, switchZ);
                Gl.glEnd();
            }

            // Рамка выключателя
            Gl.glColor3f(0.1f, 0.1f, 0.1f); 
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3f(switchX - 0.1f, switchY - 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX + switchWidth + 0.1f, switchY - 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX + switchWidth + 0.1f, switchY + switchHeight + 0.1f, switchZ - 0.01f);
            Gl.glVertex3f(switchX - 0.1f, switchY + switchHeight + 0.1f, switchZ - 0.01f);
            Gl.glEnd();
            Gl.glLineWidth(1.0f);

            Gl.glPushMatrix();
            Gl.glTranslatef(-5.0f, -4.35f + 0.5f, 1.5f); // Позиция вазы
            Gl.glRotatef(vaseRotationAngle, 1.0f, 3.0f, 3.0f); // Вращение вазы
            DrawRevolvedVase();
            Gl.glPopMatrix();

            // Рисуем "картину" с фрактальной линией (кривая дракона)
            float[] yellowColor = new float[] { 1.0f, 1.0f, 0.0f }; 
            DrawFractalDragonCurve(2.0f, 3.5f, -4.98f, 2.5f, 12, yellowColor);

            // Второй фрактал - зеленый
            float[] greenColor = new float[] { 0.0f, 1.0f, 0.0f }; 
            DrawFractalDragonCurve(-5.0f, 3.5f, -4.98f, 2.5f, 9, greenColor);
            
            DrawLion(0.0f, -4.9f, 1.0f); // Рисуем льва внутри дома
            DrawCrane(3.0f, -2.9f, 5.0f); // Рисуем журавля (парикхмахера)
            DrawTable(-5.0f, -6.0f, 1.5f); // Рисуем стол

            RenderTimer.Start();
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
            Gl.glColor3fv(color);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            foreach (PointF p in points)
            {
                Gl.glVertex3f(p.X, p.Y, z);
            }
            Gl.glEnd();

            // Увеличенная рамка для картины
            Gl.glColor3f(0.0f, 0.0f, 0.0f);
            Gl.glLineWidth(3.0f); 
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
            cloudOffsetX += cloudSpeed;

            // Возвращаем облака в начало, когда они уходят за границу
            if (cloudOffsetX > 20.0f)
                cloudOffsetX = -20.0f;

            // Анимация крыльев
            if (wingDirectionUp)
            {
                wingAngle += 2.0f;
                if (wingAngle > 30.0f) wingDirectionUp = false;
            }
            else
            {
                wingAngle -= 2.0f;
                if (wingAngle < -30.0f) wingDirectionUp = true;
            }

            vaseRotationAngle += vaseRotationSpeed;
            if (vaseRotationAngle > 360.0f)
                vaseRotationAngle -= 360.0f;

            RenderScene();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Если не в парикмахерской — выходим
            if (!isInsideHouse) return;

            isSmallHeadMode = !isSmallHeadMode; // Переключаем режим

            if (isSmallHeadMode)
            {
                lionHeadSize = 1.2f;  // Уменьшенная голова
                hasEars = true;      // Показываем ушки
            }
            else
            {
                lionHeadSize = 1.7f;  // Обычный размер
                hasEars = false;      // Скрываем ушки
            }

            RenderScene(); // Обновляем отрисовку
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Переключаем эффект резкости
            isSharpnessEffectActive = !isSharpnessEffectActive;

            if (isSharpnessEffectActive)
            {
                button2.BackColor = Color.LightGreen;
                button2.Text = "Резкость: ВКЛ";
            }
            else
            {
                button2.BackColor = SystemColors.Control;
                button2.Text = "Резкость: ВЫКЛ";
            }

            RenderScene();
        }

        private List<PointF> InterpolateLagrange(List<PointF> points, int numPoints)
        {
            List<PointF> result = new List<PointF>();

            float minY = points[0].Y;
            float maxY = points[points.Count - 1].Y;
            float step = (maxY - minY) / (numPoints - 1);

            for (int i = 0; i < numPoints; i++)
            {
                float y = minY + i * step;
                float x = LagrangePolynomial(points, y);
                result.Add(new PointF(x, y));
            }

            return result;
        }

        private float LagrangePolynomial(List<PointF> points, float y)
        {
            float result = 0.0f;
            int n = points.Count;

            for (int i = 0; i < n; i++)
            {
                float term = points[i].X;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        term *= (y - points[j].Y) / (points[i].Y - points[j].Y);
                }
                result += term;
            }

            return result;
        }

    }
}
