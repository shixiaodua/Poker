/// <summary>
/// 登录结果
/// </summary>
public enum LoginResult
{
    Success = 0,
    Fail = 1,
}
/// <summary>
/// 注册结果
/// </summary>
public enum RegisterResult
{
    Success = 0,
    Fail = 1,
    UserNameExist = 2,
}
/// <summary>
/// 验证码状态
/// </summary>
public enum YzmResult
{
    Success = 0,
    Fail = 1,
    YzmExist = 2,
}

/// <summary>
/// 创建房间结果
/// </summary>
public enum CreateRoomResult
{
    Success = 0,
    Fail = 1,
}
public enum UpdateRoomResult
{
    Success = 0,
    Fail = 1,
}
public enum RoomState
{
    waiting = 0,
    Playing = 1,
}
public enum EnterRoomResult
{
    Success = 0,
    Fail = 1,
}
public enum ExitRoomResult
{
    Success = 0,
    Fail = 1,
}
public enum ReadyResult
{
    Success = 0,
    Fail = 1,
    startGame = 2,
}
public enum RoomAudioPlayResult
{
    Success = 0,
    Fail = 1,
}
public enum StartGameResult
{
    Success = 0,
    Fail = 1,
}
public enum QdzResult
{
    Success = 0,
    Fail = 1,
}
public enum CpResult
{
    Success = 0,
    Fail = 1,
}
public enum ByResult
{
    Success = 0,
    Fail = 1,
}