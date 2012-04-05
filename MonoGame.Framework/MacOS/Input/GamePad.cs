﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GamepadConfigLib;
using Tao.Sdl;
using System.Xml.Serialization;

using MonoMac.Foundation;

namespace Microsoft.Xna.Framework.Input
{
    //
    // Summary:
    //     Allows retrieval of user interaction with an Xbox 360 Controller and setting
    //     of controller vibration motors. Reference page contains links to related
    //     code samples.
    public static class GamePad
    {
		static bool running;
		static bool sdl;
        static Settings settings;
        static Settings Settings
        {
        	get
            {
                return PrepSettings();
            }
        }

		static PadConfig Controller;
		static List<PadConfig> s_padConfigs = new List<PadConfig>();	
	    static IntPtr[] devices = new IntPtr[4];	
		
		static void InitButton( GamepadConfigLib.Input button, byte id )
		{
			button.ID = id;
			button.Type = InputType.Button;
		}

		static void InitAxis( Axis axis, byte id )
		{
			axis.Negative.ID = id;
			axis.Negative.Type = InputType.Axis;
			axis.Negative.Negative = true;
			
			axis.Positive.ID = id;
			axis.Positive.Type = InputType.Axis;
			axis.Positive.Negative = false;
		}
		
		static void InitStick( Stick stick, byte press, byte x, byte y )
		{
			InitButton( stick.Press, press  );
			InitAxis( stick.X, x );
			InitAxis( stick.Y, y );
		}
			
		
		static GamePad()
		{
			const int BUTTON_A = 11;
			const int BUTTON_B = 12;
			const int BUTTON_X = 13;
			const int BUTTON_Y = 14;
			
			const int BUTTON_DUP = 0;
			const int BUTTON_DDOWN = 1;
			const int BUTTON_DLEFT = 2;
			const int BUTTON_DRIGHT = 3;
			
			const int BUTTON_START = 4;
			const int BUTTON_BACK = 5;
			
			const int BUTTON_LSTICK = 6;
			const int BUTTON_RSTICK = 7;
			
			const int BUTTON_LBUMPER = 8;
			const int BUTTON_RBUMPER = 9;
			
			const int AXIS_LSTICK_X = 0;
			const int AXIS_LSTICK_Y = 1;			
			const int AXIS_RSTICK_X = 2;
			const int AXIS_RSTICK_Y = 3;
		
			const int AXIS_LEFTTRIG = 4;
			const int AXIS_RIGHTTRIG = 5;
			
			Controller = new PadConfig("Controller", 0);
			
			InitButton( Controller.Button_B, BUTTON_B );
			InitButton( Controller.Button_X, BUTTON_X );
			InitButton( Controller.Button_A, BUTTON_A );
			InitButton( Controller.Button_Y, BUTTON_Y );
			InitButton( Controller.Button_Back, BUTTON_BACK );
			InitButton( Controller.Button_Start, BUTTON_START );
			InitButton( Controller.Button_LB, BUTTON_LBUMPER );
			InitButton( Controller.Button_RB, BUTTON_RBUMPER );
			
			InitButton( Controller.Dpad.Up, BUTTON_DUP );
			InitButton( Controller.Dpad.Down, BUTTON_DDOWN );
			InitButton( Controller.Dpad.Left, BUTTON_DLEFT );
			InitButton( Controller.Dpad.Right, BUTTON_DRIGHT );
		
			InitStick( Controller.RightStick, BUTTON_RSTICK, AXIS_RSTICK_X, AXIS_RSTICK_Y );
			InitStick( Controller.LeftStick, BUTTON_LSTICK, AXIS_LSTICK_X, AXIS_LSTICK_Y );
			
			Controller.RightTrigger.ID = AXIS_RIGHTTRIG;
			Controller.RightTrigger.Type = InputType.Axis;
			
			Controller.LeftTrigger.ID = AXIS_LEFTTRIG;
			Controller.LeftTrigger.Type = InputType.Axis;
		}
		
		static PadConfig FindLoadedConfig( string name )
		{
			foreach( var pc in s_padConfigs )
			{
				if( pc.JoystickName == name )
					return pc;
			}
			
			return null;
		}
		
		static void AutoConfig () 
		{
			Init();
			if (!sdl)
				return;
			
			Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
			
			int numSticks = Math.Min( Sdl.SDL_NumJoysticks(), 4 );
			
			for (int x = 0; x < numSticks; x++) 
			{
				string name = Sdl.SDL_JoystickName(x);
				PadConfig pc = FindLoadedConfig( name );
				if( pc != null )
				{
					pc.ID = x;
					settings[x] = pc;
					devices[x] = Sdl.SDL_JoystickOpen( x );
					continue;
				}
			
				/*
				if ( name == "Controller" )
				{
					settings[x] = Controller;
					devices[x] = Sdl.SDL_JoystickOpen( Controller.ID );
					continue;
				}
				else if ( name == "PLAYSTATION(R)3 Controller" )
				{
					
				}
				*/
				
				pc = new PadConfig( Sdl.SDL_JoystickName(x), x );
				devices[x] = Sdl.SDL_JoystickOpen(pc.ID);

				int numbuttons = Sdl.SDL_JoystickNumButtons(devices[x]);
				Console.WriteLine("Number of buttons for joystick: " + x + " - " + numbuttons);

				for (int b = 0; b < numbuttons; b++)
				{
					//pc
				}
				//pc.Button_A = new Input();
				pc.Button_A.ID = 0;
				pc.Button_A.Type = InputType.Button;

				pc.Button_B.ID = 1;
				pc.Button_B.Type = InputType.Button;

				pc.Button_X.ID = 2;
				pc.Button_X.Type = InputType.Button;

				pc.Button_Y.ID = 3;
				pc.Button_Y.Type = InputType.Button;

				pc.Button_Back.ID = 8;
				pc.Button_Back.Type = InputType.Button;

				pc.Button_Start.ID = 9;
				pc.Button_Start.Type = InputType.Button;

				pc.Button_LB.ID = 4;
				pc.Button_LB.Type = InputType.Button;

				pc.Button_RB.ID = 5;
				pc.Button_RB.Type = InputType.Button;

				pc.LeftStick.X.Negative.Type = InputType.Axis;
				pc.LeftStick.X.Negative.Negative = true;
				pc.LeftStick.X.Positive.Type = InputType.Axis;
				pc.LeftStick.X.Positive.Negative = false;

				pc.LeftStick.Y.Negative.ID = 1;
				pc.LeftStick.Y.Negative.Type = InputType.Axis;
				pc.LeftStick.Y.Negative.Negative = true;

				pc.LeftStick.Y.Positive.ID = 1;
				pc.LeftStick.Y.Positive.Type = InputType.Axis;
				pc.LeftStick.Y.Positive.Negative = false;

				//pc.RightStick.X.Negative.Type = InputType.Axis;
				//pc.RightStick.X.Negative.Negative = true;
				//pc.RightStick.X.Positive.Type = InputType.Axis;
				//pc.RightStick.X.Positive.Negative = false;

				//pc.RightStick.Y.Negative.ID = 1;
				//pc.RightStick.Y.Negative.Type = InputType.Axis;
				//pc.RightStick.Y.Negative.Negative = true;

				//pc.RightStick.Y.Positive.ID = 1;
				//pc.RightStick.Y.Positive.Type = InputType.Axis;
				//pc.RightStick.Y.Positive.Negative = false;

				pc.Dpad.Up.ID = 0;
				pc.Dpad.Up.Type = InputType.PovUp;

				pc.Dpad.Down.ID = 0;
				pc.Dpad.Down.Type = InputType.PovDown;

				pc.Dpad.Left.ID = 0;
				pc.Dpad.Left.Type = InputType.PovLeft;

				pc.Dpad.Right.ID = 0;
				pc.Dpad.Right.Type = InputType.PovRight;

				//pc.LeftTrigger.ID = 6;
				//pc.LeftTrigger.Type = InputType.Button;

				pc.RightTrigger.ID = 7;
				pc.RightTrigger.Type = InputType.Button;

				int numaxes = Sdl.SDL_JoystickNumAxes(devices[x]);
				Console.WriteLine("Number of axes for joystick: " + x + " - " + numaxes);

				for (int a = 0; a < numaxes; a++) 
				{
					//pc.LeftStick = new Stick();
				}

				int numhats = Sdl.SDL_JoystickNumHats(devices[x]);
				Console.WriteLine("Number of PovHats for joystick: " + x + " - " + numhats);

				for (int h = 0; h < numhats; h++) 
				{
					//pc
				}
				settings[x] = pc;
			}
		}

		
        static Settings PrepSettings()
        {
            if ( settings == null )
            {
				s_padConfigs.Clear();
				string root = NSBundle.MainBundle.ResourcePath;
				LoadConfig( Path.Combine( root, "Xbox 360 Gamepad.xml" ));
				LoadConfig( Path.Combine( root, "PS3 Gamepad.xml"));
                
				try
				{
					string searchPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "Gamepads" );
					var configs = Directory.GetFiles( searchPath, "*.xml" );
					foreach( var c in configs )
					{
						LoadConfig( c );
					}
				}
				catch
				{
				}
				
                settings = new Settings();
				AutoConfig();
            }
            else if (!running)
            {
                Init();
                return settings;
            }
			
            if (!running)
                Init();
            
			return settings;
        }

		
        static void LoadConfig(string filename)
        {
            try
            {
                using (Stream s = File.OpenRead(filename))
                {
                    XmlSerializer x = new XmlSerializer(typeof(PadConfig));
                    PadConfig e = (PadConfig)x.Deserialize(s);
					s_padConfigs.Add( e );
                }
            }
            catch
            {
                return;
            }
        }


        //Inits SDL and grabs the sticks
        static void Init ()
        {
        	running = true;
			try 
			{
        		Joystick.Init ();
				sdl = true;
			}
			catch (Exception exc) 
			{
				Console.WriteLine( exc.Message );
			}

        	for (int i = 0; i < 4; i++)
            {
        		PadConfig pc = settings[i];
        		if (pc != null)
                {
        			devices[i] = Sdl.SDL_JoystickOpen (pc.ID);
				}
			}
        }
		
        //Disposes of SDL
        static void Cleanup()
        {
            Joystick.Cleanup();
            running = false;
        }

        static IntPtr GetDevice(PlayerIndex index)
        {
            return devices[(int)index];
        }

        static PadConfig GetConfig(PlayerIndex index)
        {
            return Settings[(int)index];
        }

        static Buttons StickToButtons(Vector2 stick, Buttons left, Buttons right, Buttons up, Buttons down, float DeadZoneSize)
        {
            Buttons b = (Buttons)0;

            if (stick.X > DeadZoneSize)
                b |= right;
            if (stick.X < -DeadZoneSize)
                b |= left;
            if (stick.Y > DeadZoneSize)
                b |= up;
            if (stick.Y < -DeadZoneSize)
                b |= down;

            return b;
        }
		
		static Buttons TriggerToButtons( float trigger, Buttons button, float DeadZoneSize )
		{
			Buttons b = (Buttons)0;
			
			if ( trigger > DeadZoneSize )
				b |= button;
			
			return b;
		}
		
        static Buttons ReadButtons(IntPtr device, PadConfig c, float deadZoneSize)
        {
            short DeadZone = (short)(deadZoneSize * short.MaxValue);
            Buttons b = (Buttons)0;

            if (c.Button_A.ReadBool(device, DeadZone))
                b |= Buttons.A;
            if (c.Button_B.ReadBool(device, DeadZone))
                b |= Buttons.B;
            if (c.Button_X.ReadBool(device, DeadZone))
                b |= Buttons.X;
            if (c.Button_Y.ReadBool(device, DeadZone))
                b |= Buttons.Y;

            if (c.Button_LB.ReadBool(device, DeadZone))
                b |= Buttons.LeftShoulder;
            if (c.Button_RB.ReadBool(device, DeadZone))
                b |= Buttons.RightShoulder;

            if (c.Button_Back.ReadBool(device, DeadZone))
                b |= Buttons.Back;
            if (c.Button_Start.ReadBool(device, DeadZone))
                b |= Buttons.Start;

            if (c.LeftStick.Press.ReadBool(device, DeadZone))
                b |= Buttons.LeftStick;
            if (c.RightStick.Press.ReadBool(device, DeadZone))
                b |= Buttons.RightStick;

            if (c.Dpad.Up.ReadBool(device, DeadZone))
                b |= Buttons.DPadUp;
            if (c.Dpad.Down.ReadBool(device, DeadZone))
                b |= Buttons.DPadDown;
            if (c.Dpad.Left.ReadBool(device, DeadZone))
                b |= Buttons.DPadLeft;
            if (c.Dpad.Right.ReadBool(device, DeadZone))
                b |= Buttons.DPadRight;

            return b;
        }
		
		const float DeadZoneSize = 0.27f;
		
		static float ReadTrigger( IntPtr device, GamepadConfigLib.Input trigger )
		{
			if( trigger.Type == InputType.Button )
				return trigger.ReadBool( device, (short)(DeadZoneSize * short.MaxValue) ) ? 1 : -1;
			
			return trigger.ReadFloat( device );
			
		}
		
        static GamePadState ReadState(PlayerIndex index, GamePadDeadZone deadZone)
        {

            IntPtr device = GetDevice(index);
            PadConfig c = GetConfig(index);
            if (device == IntPtr.Zero || c == null)
                return GamePadState.InitializedState;

            GamePadThumbSticks sticks = new GamePadThumbSticks(new Vector2(c.LeftStick.ReadAxisPair(device)), new Vector2(c.RightStick.ReadAxisPair(device)));
            sticks.ApplyDeadZone(deadZone, DeadZoneSize);
			
			float leftTrigger = ReadTrigger( device, c.LeftTrigger );
			float rightTrigger = ReadTrigger( device, c.RightTrigger );
            GamePadTriggers triggers = new GamePadTriggers( leftTrigger, rightTrigger );
			
			Buttons buttonState = ReadButtons( device, c, DeadZoneSize );
			buttonState |= StickToButtons( sticks.Left, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, DeadZoneSize );
			buttonState |= StickToButtons( sticks.Right, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp, Buttons.RightThumbstickDown, DeadZoneSize );
			buttonState |= TriggerToButtons( triggers.Left, Buttons.LeftTrigger, DeadZoneSize );
			buttonState |= TriggerToButtons( triggers.Right, Buttons.RightTrigger, DeadZoneSize );
			
			GamePadButtons buttons = new GamePadButtons( buttonState );
            GamePadDPad dpad = new GamePadDPad( buttonState );

            GamePadState g = new GamePadState(sticks, triggers, buttons, dpad);
            return g;
        }

        //
        // Summary:
        //     Retrieves the capabilities of an Xbox 360 Controller.
        //
        // Parameters:
        //   playerIndex:
        //     Index of the controller to query.
        public static GamePadCapabilities GetCapabilities(PlayerIndex playerIndex)
        {
            IntPtr d = GetDevice(playerIndex);
            PadConfig c = GetConfig(playerIndex);

            if (c == null || ((c.JoystickName == null || c.JoystickName == string.Empty) && d == IntPtr.Zero))
                return new GamePadCapabilities();

            return new GamePadCapabilities()
            {
                IsConnected = d != IntPtr.Zero,
                HasAButton = c.Button_A.Type != InputType.None,
                HasBButton = c.Button_B.Type != InputType.None,
                HasXButton = c.Button_X.Type != InputType.None,
                HasYButton = c.Button_Y.Type != InputType.None,
                HasBackButton = c.Button_Back.Type != InputType.None,
                HasStartButton = c.Button_Start.Type != InputType.None,
                HasDPadDownButton = c.Dpad.Down.Type != InputType.None,
                HasDPadLeftButton = c.Dpad.Left.Type != InputType.None,
                HasDPadRightButton = c.Dpad.Right.Type != InputType.None,
                HasDPadUpButton = c.Dpad.Up.Type != InputType.None,
                HasLeftShoulderButton = c.Button_LB.Type != InputType.None,
                HasRightShoulderButton = c.Button_RB.Type != InputType.None,
                HasLeftStickButton = c.LeftStick.Press.Type != InputType.None,
                HasRightStickButton = c.RightStick.Press.Type != InputType.None,
                HasLeftTrigger = c.LeftTrigger.Type != InputType.None,
                HasRightTrigger = c.RightTrigger.Type != InputType.None,
                HasLeftXThumbStick = c.LeftStick.X.Type != InputType.None,
                HasLeftYThumbStick = c.LeftStick.Y.Type != InputType.None,
                HasRightXThumbStick = c.RightStick.X.Type != InputType.None,
                HasRightYThumbStick = c.RightStick.Y.Type != InputType.None,

                HasLeftVibrationMotor = false,
                HasRightVibrationMotor = false,
                HasVoiceSupport = false,
                HasBigButton = false
            };
        }
        //
        // Summary:
        //     Gets the current state of a game pad controller. Reference page contains
        //     links to related code samples.
        //
        // Parameters:
        //   playerIndex:
        //     Player index for the controller you want to query.
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState(playerIndex, GamePadDeadZone.IndependentAxes);
        }
        //
        // Summary:
        //     Gets the current state of a game pad controller, using a specified dead zone
        //     on analog stick positions. Reference page contains links to related code
        //     samples.
        //
        // Parameters:
        //   playerIndex:
        //     Player index for the controller you want to query.
        //
        //   deadZoneMode:
        //     Enumerated value that specifies what dead zone type to use.
        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZoneMode)
        {
            PrepSettings();
            if (sdl)
				Sdl.SDL_JoystickUpdate();
            return ReadState(playerIndex, deadZoneMode);
        }
        //
        // Summary:
        //     Sets the vibration motor speeds on an Xbox 360 Controller. Reference page
        //     contains links to related code samples.
        //
        // Parameters:
        //   playerIndex:
        //     Player index that identifies the controller to set.
        //
        //   leftMotor:
        //     The speed of the left motor, between 0.0 and 1.0. This motor is a low-frequency
        //     motor.
        //
        //   rightMotor:
        //     The speed of the right motor, between 0.0 and 1.0. This motor is a high-frequency
        //     motor.
        public static bool SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
            return false;
        }
    }
}