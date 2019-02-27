using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;


namespace Nez
{
	/// <summary>
	/// this class exists only so that we can sneak the Batcher through and have it work just like SpriteBatch with regard to resource handling.
	/// </summary>
	public abstract class GraphicsResource : IDisposable
	{
		public GraphicsDevice graphicsDevice
		{
			get
			{
				return _graphicsDevice;
			}
			internal set
			{
				Insist.isTrue( value != null );

				if( _graphicsDevice == value )
					return;

				// VertexDeclaration objects can be bound to multiple GraphicsDevice objects
				// during their lifetime. But only one GraphicsDevice should retain ownership.
				if( _graphicsDevice != null )
				{
					updateResourceReference( false );
					_selfReference = null;
				}

				_graphicsDevice = value;

				_selfReference = new WeakReference( this );
				updateResourceReference( true );
			}
		}
			
		public bool isDisposed { get; private set; }

		// The GraphicsDevice property should only be accessed in Dispose(bool) if the disposing
		// parameter is true. If disposing is false, the GraphicsDevice may or may not be disposed yet.
		GraphicsDevice _graphicsDevice;

		WeakReference _selfReference;


		internal GraphicsResource()
		{}


		~GraphicsResource()
		{
			// Pass false so the managed objects are not released
			Dispose( false );
		}


		public void Dispose()
		{
			// Dispose of managed objects as well
			Dispose( true );
			// Since we have been manually disposed, do not call the finalizer on this object
			GC.SuppressFinalize( this );
		}


		/// <summary>
		/// The method that derived classes should override to implement disposing of managed and native resources.
		/// </summary>
		/// <param name="disposing">True if managed objects should be disposed.</param>
		/// <remarks>Native resources should always be released regardless of the value of the disposing parameter.</remarks>
		protected virtual void Dispose( bool disposing )
		{
			if( !isDisposed )
			{
				if( disposing )
				{
					// Release managed objects
				}
					
				// Remove from the global list of graphics resources
				if( graphicsDevice != null )
					updateResourceReference( false );

				_selfReference = null;
				_graphicsDevice = null;
				isDisposed = true;
			}
		}


		void updateResourceReference( bool shouldAdd )
		{
			var method = shouldAdd ? "AddResourceReference" : "RemoveResourceReference";
			var methodInfo = ReflectionUtils.getMethodInfo( graphicsDevice, method );
			methodInfo.Invoke( graphicsDevice, new object[] { _selfReference } );
		}

	}
}
