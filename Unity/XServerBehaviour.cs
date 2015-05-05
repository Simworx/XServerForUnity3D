using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mustache;
using System.Linq;

[RequireComponent(typeof(XServerBehaviour))]
public class ViewMasterController : MonoBehaviour {

	//
	private bool _keyResetRequired = true;
	private XServerBehaviour _webServer = null;
	
	// Use this for initialization
	void Start () {
		_webServer = GetComponent<XServerBehaviour> ();
		_webServer.RegisterController (new LobbyController   (_webServer, this));
		_webServer.RegisterController (new IMTController     (_webServer, this));
		_webServer.RegisterController (new TheatreController (_webServer, this));
		_webServer.RegisterController (new CommandsController(_webServer, this));
	}
	
	// Update is called once per frame
	void Update () {
		if (_keyResetRequired) {
			_keyResetRequired = Input.anyKeyDown; // will set to false once all keys are released.
		} else {
			if (Input.GetKeyDown (KeyCode.Space)) {
				OVRManager.display.RecenterPose ();
				_keyResetRequired = true;
			}
		}
	}

	public ControllerContext GetWebContext()
	{
		return new ControllerContext ();
	}

	public string RenderView(ControllerContext ctx, string toRender)
	{
		var fc = new FormatCompiler();
		fc.RegisterTag(new IncludeTag(this, ctx, _webServer.StaticFileController), true);
		var generator = fc.Compile(toRender);
		return generator.Render(ctx);
	}
}

public abstract class VMController : XServer.Controller
{
	public readonly XServerBehaviour Server;
	public readonly ViewMasterController ViewMasterController;
	
	public VMController(string route, XServerBehaviour server, ViewMasterController vmc, bool exactRoute = true)
		: base(route, exactRoute)
	{
		Server = server;
		ViewMasterController = vmc;
	}
}
