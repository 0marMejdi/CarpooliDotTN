using Microsoft.AspNetCore.Html;
using Microsoft.VisualBasic;

namespace CarpooliDotTN.Models;

public class FlashMessage
{
    private static string? _message;
    private static string? _color;
    public static bool IsEmpty=true;
    
    private static void Send(string? message)
    {
        IsEmpty = false;
        _message = message;
    }
    
    public static void SendError(string? message)
    {
        Send(message);
        _color = "alert-danger";

    }

    public static void SendSuccess(string? message)
    {
        Send(message);
        _color = "alert-success";
    }

    public static void SendInfo(string? message)
    {
        Send(message);
        _color = "alert-info";
    }

    public static string? GetMessage()
    {
        string? temp = _message;
        _message = "";

        if (IsEmpty)
        { 
            return "";
        }
        IsEmpty = true;
        return temp;
    }

    public static string? GetColor()
    {
        return _color;
    }
}