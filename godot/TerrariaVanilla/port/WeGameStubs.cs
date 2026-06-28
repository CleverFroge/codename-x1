using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.Net;
using Terraria.Net.Sockets;
using Terraria.Social.Base;

namespace Terraria.Social.WeGame;

public static class WeGameHelper
{
	public static void WriteDebugString(string format, params object[] args)
	{
	}
}

public class CoreSocialModule : ISocialModule
{
	public void Initialize()
	{
	}

	public void Shutdown()
	{
	}
}

public class CloudSocialModule : Terraria.Social.Base.CloudSocialModule
{
	public override void Initialize()
	{
	}

	public override void Shutdown()
	{
	}

	public override IEnumerable<string> GetFiles() => Array.Empty<string>();

	public override bool Write(string path, byte[] data, int length) => false;

	public override void Read(string path, byte[] buffer, int length)
	{
	}

	public override bool HasFile(string path) => false;

	public override int GetFileSize(string path) => 0;

	public override bool Delete(string path) => false;

	public override bool Forget(string path) => false;
}

public class FriendsSocialModule : Terraria.Social.Base.FriendsSocialModule
{
	public override string GetUsername() => "";

	public override void OpenJoinInterface()
	{
	}

	public override void Initialize()
	{
	}

	public override void Shutdown()
	{
	}
}

public class OverlaySocialModule : Terraria.Social.Base.OverlaySocialModule
{
	public override void Initialize()
	{
	}

	public override void Shutdown()
	{
	}

	public override bool IsGamepadTextInputActive() => false;

	public override bool ShowGamepadTextInput(string description, uint maxLength, bool multiLine = false, string existingText = "", bool password = false) => false;

	public override string GetGamepadText() => "";
}

public class NetClientSocialModule : NetSocialModule
{
	public override void Initialize()
	{
	}

	public override void Shutdown()
	{
	}

	public override void Close(RemoteAddress address)
	{
	}

	public override bool IsConnected(RemoteAddress address) => false;

	public override void Connect(RemoteAddress address)
	{
	}

	public override bool Send(RemoteAddress address, byte[] data, int length) => false;

	public override int Receive(RemoteAddress address, byte[] data, int offset, int length) => 0;

	public override bool IsDataAvailable(RemoteAddress address) => false;

	public override void LaunchLocalServer(Process process, ServerMode mode)
	{
	}

	public override bool CanInvite() => false;

	public override void OpenInviteInterface()
	{
	}

	public override void CancelJoin()
	{
	}

	public override bool StartListening(SocketConnectionAccepted callback) => false;

	public override void StopListening()
	{
	}

	public override ulong GetLobbyId() => 0;
}

public class NetServerSocialModule : NetClientSocialModule
{
}
