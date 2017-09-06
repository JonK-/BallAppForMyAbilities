using System;
using System.Collections.Generic;
using CocosSharp;

namespace BouncingGame
{
	public class GameLayer : CCLayerColor
	{
        private CCDrawNode _targetZone;
        private CCDrawNode _line;
        private CCDrawNode _ball;

        private CCPoint _lastLineStartPoint;

        private CCParticleSun _glow;
        private CCParticleExplosion _burst;

        private CCRect _bounds;

        private const float LINE_WIDTH = 2.5f;

        public GameLayer () : base (CCColor4B.Black)
		{
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			_bounds = VisibleBoundsWorldspace;

            //var background = new CCSprite("background");
            //AddChild(background);

			_targetZone = new CCDrawNode();
			_targetZone.Position = new CCPoint(_bounds.MaxX - 100, _bounds.MaxY - 100);
			_targetZone.DrawRect(new CCPoint(0, 0), 50, CCColor4B.Transparent);
			AddChild(_targetZone);

			var coinSprite = new CCSprite("coin");
			_targetZone.AddChild(coinSprite);

			_line = new CCDrawNode();
			AddChild(_line);

			_ball = new CCDrawNode();
			_ball.Position = new CCPoint(100, 100);
            _ball.DrawCircle(new CCPoint(0,0), 50, CCColor4B.Transparent);
			AddChild(_ball);
            ReorderChild(_ball, 2);

            var ballSprite = new CCSprite("ball");
			_ball.AddChild(ballSprite);
            //ballSprite.Scale = 0.15f;
			_ball.ReorderChild(ballSprite, 2);

			_glow = new CCParticleSun(new CCPoint(0, 0), CCEmitterMode.Radius);
			_glow.StartColor = new CCColor4F(CCColor3B.Orange);
			_glow.EndColor = new CCColor4F(CCColor3B.Yellow);
            _glow.StartRadius = _ball.ContentSize.Width * 0.4f;
            _glow.EndRadius = _ball.ContentSize.Width * 0.5f;

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
                hasReachedTargetZone = false;
                ReachedTargetZone();
            }
		}

		void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
                _line.DrawLine(touches[0].StartLocation, touches[0].Location, LINE_WIDTH, CCColor4B.Yellow, CCLineCap.Round);

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

                _line.DrawLine(_lastLineStartPoint, touches[0].Location, LINE_WIDTH, CCColor4B.Yellow, CCLineCap.Round);

                _lastLineStartPoint = touches[0].Location;

                if(_targetZone.BoundingBoxTransformedToParent.ContainsPoint(touches[0].Location))
                {
                    ReachedTargetZone();
                }
			}
		}

        private void ReachedTargetZone ()
        {
			_burst = new CCParticleExplosion(_targetZone.Position, CCEmitterMode.Gravity);
			_burst.Speed = 75;
			AddChild(_burst);

			Random rnd = new Random();
			int x = rnd.Next(100, (int)_bounds.MaxX - 100);
			int y = rnd.Next(100, (int)_bounds.MaxY - 100);

			_targetZone.Position = new CCPoint(x, y);

			RemoveChild(_line);

			_line = new CCDrawNode();
			AddChild(_line);
        }
	}
}
