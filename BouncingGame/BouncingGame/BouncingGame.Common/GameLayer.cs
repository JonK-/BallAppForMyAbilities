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

        private CCDrawNode _line;
        private CCDrawNode _ball;

        private CCPoint _lastLineStartPoint;


        public GameLayer () : base (CCColor4B.Gray)
		{
			_line = new CCDrawNode();
			AddChild(_line);

            _ball = new CCDrawNode();
            _ball.Position = new CCPoint(100, 100);
            AddChild(_ball);

			var glow = new CCParticleSun(new CCPoint(0, 0), CCEmitterMode.Radius);
			glow.StartColor = new CCColor4F(CCColor3B.Orange);
			glow.EndColor = new CCColor4F(CCColor3B.Yellow);
			glow.StartRadius = 45;
			glow.EndRadius = 50;
            _ball.AddChild(glow);

			var ballFill = new CCDrawNode();
			ballFill.DrawSolidCircle(new CCPoint(0, 0), 50, CCColor4B.Red);
			_ball.AddChild(ballFill);

			Schedule (RunGameLogic);
		}

		void RunGameLogic(float frameTimeInSeconds)
		{
			//// Check if the two CCSprites overlap...
			//bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(
			//	paddleSprite.BoundingBoxTransformedToParent);


		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesBegan = OnTouchesBegan;
			touchListener.OnTouchesEnded = OnTouchesEnded;
			touchListener.OnTouchesMoved = HandleTouchesMoved;
			AddEventListener (touchListener, _ball);
		}

		void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
                _line.DrawLine(touches[0].StartLocation, touches[0].Location);

                _lastLineStartPoint = new CCPoint(touches[0].Location);
			}
		}

		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
                var burst = new CCParticleExplosion(new CCPoint(0,0), CCEmitterMode.Gravity);
				burst.Speed = 100;
				_ball.AddChild(burst);
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
