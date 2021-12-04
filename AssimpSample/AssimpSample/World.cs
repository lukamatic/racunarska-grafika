// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 10000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.Enable(OpenGL.GL_DEPTH_TEST); // ukljuceno testiranje dubine LM
            gl.Enable(OpenGL.GL_CULL_FACE); // ukljuceno sakrivanje nevidiljivih povrsina LM
            gl.ShadeModel(OpenGL.GL_FLAT);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Viewport(0, 0, m_width, m_height); // redefinisanje viewporta koji se koristi za iscrtavanje modela

            // iscrtavanje elemenata scene
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            // iscrtavanje kamiona
            gl.PushMatrix();
            gl.Translate(-3500.0f, 0.0f, 3000.0f);
            gl.Scale(0.01, 0.01, 0.01);
            gl.Rotate(180, 0.0f, 1.0f, 0.0f);
            m_scene.Draw();
            gl.PopMatrix();

            // iscrtavanje podloge
            gl.PushMatrix();
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(1.0f, 1.0f, 0.3f);
            gl.Vertex(-10000.0f, -1700.0f, -10000.0f);
            gl.Vertex(-10000.0f, -1700.0f, 10000.0f);
            gl.Vertex(10000.0f, -1700.0f, 10000.0f);
            gl.Vertex(10000.0f, -1700.0f, -10000.0f);
            gl.End();
            gl.PopMatrix();

            // iscrtavanje ulice
            gl.PushMatrix();
            gl.Color(65.0f / 255, 65.0f / 255, 65.0f / 255);

            // prvi deo ulice
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-3000.0f, -1680.0f, -2000.0f);
            gl.Vertex(-3000.0f, -1680.0f, 5000.0f);
            gl.Vertex(-1000.0f, -1680.0f, 5000.0f);
            gl.Vertex(-1000.0f, -1680.0f, -2000.0f);
            gl.End();

            // drugi deo ulice 
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-3000.0f, -1680.0f, -4000.0f);
            gl.Vertex(-3000.0f, -1680.0f, -2000.0f);
            gl.Vertex(5000.0f, -1680.0f, -2000.0f);
            gl.Vertex(5000.0f, -1680.0f, -4000.0f);
            gl.End();

            // kraj iscrtavanje ulice
            gl.PopMatrix();

            // iscrtavanje zidova gradilista
            var cube = new Cube();
            gl.Color(85.0f / 255, 65.0f / 255, 25.0f / 255);

            gl.PushMatrix();
            gl.Translate(4400.0f, -1000.0f, 0.0f);
            gl.Scale(400, 1000, 2000);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(4400.0f, -1000.0f, -6000.0f);
            gl.Scale(400, 1000, 2000);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(7600.0f, -1000.0f, -3000.0f);
            gl.Scale(400, 1000, 5000);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(85.0f / 255, 65.0f / 255, 25.0f / 255);
            gl.Translate(6000.0f, -1000.0f, 2400.0f);
            gl.Scale(2000, 1000, 400);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(85.0f / 255, 65.0f / 255, 25.0f / 255);
            gl.Translate(6000.0f, -1000.0f, -8400.0f);
            gl.Scale(2000, 1000, 400);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // iscrtavanje rampe
            gl.Color(1f, 0f, 0f);

            // iscrtavanje levog stubica rampe
            gl.PushMatrix();
            gl.Translate(3200.0f, -1600.0f, -4200.0f);
            gl.Scale(100, 200, 100);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // iscrtavanje desnog stubica rampe
            gl.PushMatrix();
            gl.Translate(3200.0f, -1600.0f, -1800.0f);
            gl.Scale(100, 200, 100);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // iscrtavanje precke rampe
            var cylinder = new Cylinder();
            gl.Translate(3200.0f, -1400.0f, -4200.0f);
            gl.Scale(100, 100, 2400);
            cylinder.TopRadius = 0.3f;
            cylinder.BaseRadius = 0.3f;
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // kraj iscrtavanja elemenata scene
            gl.PopMatrix();

            // ispisivanje teksta
            gl.Viewport(m_width * 4 / 5, 0, m_width / 5, m_height / 2);
            gl.DrawText3D("Verdana", 14, 0, 0, "");
            gl.DrawText(0, 200, 1f, 0f, 0f, "Verdana", 14, "Predmet: Racunarska grafika");
            gl.DrawText(0, 165, 1f, 0f, 0f, "Verdana", 14, "Sk.god: 2021/22");
            gl.DrawText(0, 130, 1f, 0f, 0f, "Verdana", 14, "Ime: Luka");
            gl.DrawText(0, 95, 1f, 0f, 0f, "Verdana", 14, "Prezime: Matic");
            gl.DrawText(0, 60, 1f, 0f, 0f, "Verdana", 14, "Sifra zad: 5.2");

            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50.0f, (double)m_width / m_height, 1f, 50000f); // definisanje projekcije u perspektivi LM
            gl.Viewport(0, 0, m_width, m_height); // postavljanje viewporta preko celog ekrana
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
