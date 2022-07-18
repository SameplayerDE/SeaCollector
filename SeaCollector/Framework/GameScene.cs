using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Entities;
using SeaCollector.Rendering;

namespace SeaCollector.Framework
{
    public class GameScene
    {
        public string SceneName { get; private set; }
        protected ContentManager Content;
        public Game Game;
        public List<GameObject2D> SceneObjects2D { get; private set; }
        public List<GameObject3D> SceneObjects3D { get; private set; }

        public GameScene(string name, Game game)
        {
            SceneName = name;
            Game = game;
            SceneObjects2D = new List<GameObject2D>();
            SceneObjects3D = new List<GameObject3D>();
            Content = new ContentManager(Game.Services, "Content");
        }

        public override bool Equals(object obj)
        {
            if (obj is GameScene)
            {
                return SceneName.Equals((obj as GameScene).SceneName);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AddSceneObject(GameObject2D sceneObject)
        {
            if (!SceneObjects2D.Contains(sceneObject))
            {
                sceneObject.Scene = this;
                SceneObjects2D.Add(sceneObject);
            }
        }

        public void RemoveSceneObject(GameObject2D sceneObject)
        {
            if (SceneObjects2D.Remove(sceneObject))
            {
                sceneObject.Scene = null;
            }
        }
        
        public void AddSceneObject(GameObject3D sceneObject)
        {
            if (SceneObjects3D.Contains(sceneObject))
                return;
            SceneObjects3D.Add(sceneObject);
        }

        public void RemoveSceneObject(GameObject3D sceneObject)
        {
            if (!SceneObjects3D.Remove(sceneObject))
            {
                return;
            }
        }

        public virtual void Activated(){}
        public virtual void Deactivated(){}

        public virtual void Initialize()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.Initialize());
            SceneObjects3D.ForEach(sceneObject => sceneObject.Initialize());
        }

        public virtual void LoadContent()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.LoadContent(Content));
            SceneObjects3D.ForEach(sceneObject => sceneObject.LoadContent(Game.GraphicsDevice, Content));
        }
        
        public virtual void UnloadContent()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.UnloadContent());
            SceneObjects3D.ForEach(sceneObject => sceneObject.UnloadContent());
        }

        public virtual void Update(GameTime gameTime)
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.Update(gameTime));
            SceneObjects3D.ForEach(sceneObject => sceneObject.Update(gameTime));
        }

        public virtual void Draw2D(RenderContext renderContext, bool drawInFrontOf3D = false)
        {
            SceneObjects2D.ForEach(obj =>
            {
                if (obj.DrawInFrontOf3D == drawInFrontOf3D)
                    obj.Draw(renderContext);
            });
        }
        
        public virtual void Draw3D(RenderContext renderContext)
        {
            SceneObjects3D.ForEach((Action<GameObject3D>)(sceneObject =>
                sceneObject.Draw(renderContext)));
        }
    }
}