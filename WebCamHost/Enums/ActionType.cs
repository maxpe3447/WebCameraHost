namespace WebCamHost.Enums
{
    public enum ActionType : byte
    {
        NONE,
        REGISTRATION,
        START_RECORD,
        STOP_RECORD,
        SEARCH_PEOPLE,
        SELECT_CAMERA,
        START_SHOW,
        STOP_SHOW,
        GET_CAMERAS,
        DISCONNECT,
        CAMERA_CHANGE
    }
}
