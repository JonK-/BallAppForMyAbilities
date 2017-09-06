using System;
using System.Collections.Generic;
using CocosSharp;

namespace BouncingGame
{
	public class GameLayer : CCLayerColor
	{
        //CCSprite paddleSprite;
        //CCSprite ballSprite;
        //CCLabel scoreLabel;

        //float ballXVelocity;
        //float ballYVelocity;

        //// How much to modify the ball's y velocity per second:
        //const float gravity = 140;

        //int score;

        private CCDrawNode _targetZone;
        private CCDrawNode _line;
        private CCDrawNode _ball;

        private CCPoint _lastLineStartPoint;

        private CCParticleSun _glow;
        private CCParticleExplosion _burst;

        private CCRect _bounds;


        public GameLayer () : base (CCColor4B.Gray)
		{
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			_bounds = VisibleBoundsWorldspace;

			_targetZone = new CCDrawNode();
			_targetZone.Position = new CCPoint(_bounds.MaxX - 100, _bounds.MaxY - 100);
			_targetZone.DrawRect(new CCPoint(0, 0), 50, CCColor4B.Blue);
			AddChild(_targetZone);

			_line = new CCDrawNode();
			AddChild(_line);

			_ball = new CCDrawNode();
			_ball.Position = new CCPoint(100, 100);
			AddChild(_ball);

			_glow = new CCParticleSun(new CCPoint(0, 0), CCEmitterMode.Radius);
			_glow.StartColor = new CCColor4F(CCColor3B.Orange);
			_glow.EndColor = new CCColor4F(CCColor3B.Yellow);
			_glow.StartRadius = 45;
			_glow.EndRadius = 50;

			var ballFill = new CCDrawNode();
			ballFill.DrawSolidCircle(new CCPoint(0, 0), 50, CCColor4B.Red);
			_ball.AddChild(ballFill);
			_ball.ReorderChild(ballFill, 2);

			Schedule(RunGameLogic);

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesBegan = OnTouchesBegan;
			touchListener.OnTouchesEnded = OnTouchesEnded;
			touchListener.OnTouchesMoved = HandleTouchesMoved;
			AddEventListener (touchListener, _ball);
		}

		void RunGameLogic(float frameTimeInSeconds)
		{
            bool hasReachedTargetZone = _ball.BoundingBoxTransformedToParent.IntersectsRect(_targetZone.BoundingBoxTransformedToParent);

            if(hasReachedTargetZone)
            {
				_burst = new CCParticleExplosion(new CCPoint(0, 0), CCEmitterMode.Gravity);
				_burst.Speed = 150;
				_ball.AddChild(_burst);

				Random rnd = new Random();
                int x = rnd.Next(100, (int)_bounds.MaxX - 100);
                int y = rnd.Next(100, (int)_bounds.MaxY - 100);

                _targetZone.Position = new CCPoint(x, y);

                hasReachedTargetZone = false;
            }
		}

		void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
                _line.DrawLine(touches[0].StartLocation, touches[0].Location);

                _lastLineStartPoint = new CCPoint(touches[0].Location);

				_ball.AddChild(_glow);
                _ball.ReorderChild(_glow, 1);
			}
		}

		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
                _ball.RemoveChild(_glow);
			}
		}

		void HandleTouchesMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
            if (touches.Count > 0)
            {
                _ball.Position = touches[0].Location;

                _line.DrawLine(_lastLineStartPoint, touches[0].Location);

                _lastLineStartPoint = touches[0].Location;
			}
		}
	}
}
