/// <summary>
/// <para>Commonly used player input names, unity is bad about this, some are strings, some with spaces, some are numbers
/// <br />
/// If you need to use it more than once, I'd put it here.</para>
/// <para>If adding a new constant, <i>the constant will not work</i> with dynamic inputs from the player like regularly
/// changing axes
/// <br />
/// such as Input.mouseScrollDelta.x, or getting current mouse position.</para>
/// </summary>
public static class PlayerControllerConstants
{
    public static readonly int LeftMouseButton = 0;
    public static readonly int RightMouseButton = 1;
    /// <summary>
    /// If mouse is <b>moving</b>, its direction relative to 0, negative is left, positive is right.
    /// </summary>
    public static readonly string MouseXAxis = "Mouse X";
    /// <summary>
    /// If mouse is <b>moving</b>, its direction relative to 0, negative is down, positive is up.
    /// </summary>
    public static readonly string MouseYAxis = "Mouse Y";
}


