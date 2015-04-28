using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

public class XServerBehaviour : MonoBehaviour {

    internal class RootController : XServer.Controller
    {
        readonly XServer.StaticFileController _sfc = null;
        public RootController(XServer.StaticFileController sfc)
            : base("/", true)
        {
            _sfc = sfc;
        }

        public override void OnConnection(XServer.HttpRequest request, XServer.HttpResponse response)
        {
            var contents = _sfc.ReadFile("index.html");
            if (contents != null)
            {
                response.SetBody(contents);
            }
            else
            {
                response.SetCode(404);
            }
        }
    }

	/// <summary>
	/// The webserver
	/// </summary>
	XServer.WebServer ws = null;

	/// <summary>
	/// The location of the 'www' directory.
	/// </summary>
	public string[] _fileDirectories = new string[] { "{{CWD}}/Assets/WWW" };

	/// <summary>
	/// The port we want the webserver to run on.
	/// </summary>
	public int _port = 80;

	/// <summary>
	/// List of controllers to be registered post creation
	/// </summary>
	private List<XServer.Controller> _deferredRegister;

	// Use this for initialization
	void Start () {

        var sfc = new XServer.StaticFileController(_fileDirectories);
        var rc = new RootController(sfc);

		// start the server
		XServer.WebServer ws = new XServer.WebServer(_port, false); //gzip causes trouble in Unity
        ws.RegisterController(rc);
        ws.RegisterController(sfc);
		if (_deferredRegister != null) {
			foreach (var c in _deferredRegister) {
					ws.RegisterController (c);
			}
			_deferredRegister = null;
		}
		ws.Start();

		//
		Debug.Log ("Web server is now serving on port " + _port.ToString());
	}

	/// <summary>
	/// Registers a controller.
	/// </summary>
	/// <param name="c">C.</param>
	public void RegisterController(XServer.Controller c)
	{
		if (ws != null) {
			ws.RegisterController (c);
		} else {
			if(_deferredRegister == null){
				_deferredRegister = new List<XServer.Controller>();
			}
			_deferredRegister.Add(c);
		}
	}

	/// <summary>
	/// Shutdown the webserver.
	/// </summary>
	public void Shutdown()
	{
		if (ws != null) 
		{
			ws.Stop ();
			ws = null;
		}
	}

	/// <summary>
	/// Raised at the destroy event.
	/// </summary>
	void OnDestroy()
	{
		Shutdown ();
	}

	/// <summary>
	/// Raised at the application quit event.
	/// </summary>
	void OnApplicationQuit()
	{
		Shutdown ();
	}
}
