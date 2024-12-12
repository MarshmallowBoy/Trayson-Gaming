namespace TND.XeSS
{
    public enum XeSSResult
    {
        /** Warning. Folder to store dump data doesn't exist. Write operation skipped.*/
        XESS_RESULT_WARNING_NONEXISTING_FOLDER = 1,
        /** An old or outdated driver. */
        XESS_RESULT_WARNING_OLD_DRIVER = 2,
        /** X<sup>e</sup>SS operation was successful. */
        XESS_RESULT_SUCCESS = 0,
        /** X<sup>e</sup>SS not supported on the GPU. An SM 6.4 capable GPU is required. */
        XESS_RESULT_ERROR_UNSUPPORTED_DEVICE = -1,
        /** An unsupported driver. */
        XESS_RESULT_ERROR_UNSUPPORTED_DRIVER = -2,
        /** Execute called without initialization. */
        XESS_RESULT_ERROR_UNINITIALIZED = -3,
        /** Invalid argument such as descriptor handles. */
        XESS_RESULT_ERROR_INVALID_ARGUMENT = -4,
        /** Not enough available GPU memory. */
        XESS_RESULT_ERROR_DEVICE_OUT_OF_MEMORY = -5,
        /** Device function such as resource or descriptor creation. */
        XESS_RESULT_ERROR_DEVICE = -6,
        /** The function is not implemented */
        XESS_RESULT_ERROR_NOT_IMPLEMENTED = -7,
        /** Invalid context. */
        XESS_RESULT_ERROR_INVALID_CONTEXT = -8,
        /** Operation not finished yet. */
        XESS_RESULT_ERROR_OPERATION_IN_PROGRESS = -9,
        /** Operation not supported in current configuration. */
        XESS_RESULT_ERROR_UNSUPPORTED = -10,
        /** The library cannot be loaded. */
        XESS_RESULT_ERROR_CANT_LOAD_LIBRARY = -11,

        /** Unknown internal failure */
        XESS_RESULT_ERROR_UNKNOWN = -1000,

        //Custom error denoting that no xess backend could be found on the current graphics backend
        XESS_RESULT_ERROR_GRAPHICS_DEVICE_INCOMPATIBLE = -100,
    }
}
