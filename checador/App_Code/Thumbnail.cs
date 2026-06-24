using System;
using System.Drawing;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace checador
{

    /// <summary>
    /// Summary description for Thumbnail
    /// </summary>
    public class Thumbnail
    {
        protected string _url;
        protected int _width, _height;
        protected int _thumbWidth, _thumbHeight;
        protected Bitmap _bmp;
        public Thumbnail()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Generates a website thumbnail for the given URL
        /// </summary>
        /// <param name="url">Address of website from which to generate the
        /// thumbnail</param>
        /// <param name="width">Browser width</param>
        /// <param name="height">Browser height</param>
        /// <param name="thumbWidth">Width of generated thumbnail</param>
        /// <param name="thumbHeight">Height of generated thumbnail</param>
        /// <returns></returns>
        public static Bitmap GetThumbnail(string url, int width, int height,
          int thumbWidth, int thumbHeight)
        {
            Thumbnail thumbnail = new Thumbnail(url, width, height,
              thumbWidth, thumbHeight);
            return thumbnail.GetThumbnail();
        }
        /// <summary>
	/// Protected constructor
	/// </summary>
    protected Thumbnail(string url, int width, int height,
	  int thumbWidth, int thumbHeight)
	{
		_url = url;
		_width = width;
		_height = height;
		_thumbWidth = thumbWidth;
		_thumbHeight = thumbHeight;
	}

	/// <summary>
	/// Returns a thumbnail for the current member values
	/// </summary>
	/// <returns>Thumbnail bitmap</returns>
	protected Bitmap GetThumbnail()
	{
		// WebBrowser is an ActiveX control that must be run in a
		// single-threaded apartment so create a thread to create the
		// control and generate the thumbnail
		Thread thread = new Thread(new ThreadStart(GetThumbnailWorker));
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();
        return _bmp;
        //return _bmp.GetThumbnailImage(_thumbWidth, _thumbHeight,
        //  null, IntPtr.Zero) as Bitmap;
	}

	/// <summary>
	/// Creates a WebBrowser control to generate the thumbnail image
	/// Must be called from a single-threaded apartment
	/// </summary>
	protected void GetThumbnailWorker()
	{
		using (WebBrowser browser = new WebBrowser())
		{
			browser.ClientSize = new Size(_width, _height);
			browser.ScrollBarsEnabled = false;
			browser.ScriptErrorsSuppressed = true;
			browser.Navigate(_url);

			// Wait for control to load page
			while (browser.ReadyState != WebBrowserReadyState.Complete)
				Application.DoEvents();

			// Render browser content to bitmap
			_bmp = new Bitmap(_width, _height);
			browser.DrawToBitmap(_bmp, new Rectangle(0, 0,
			  _width, _height));
		}
	}
    }
}