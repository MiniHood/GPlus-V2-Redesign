using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceProcess;
using System.Text;
using GPlus_V2_Redesign.Source;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

// All credit to saykowa for this amazing Sandboxie wrapper.
// https://github.com/saykowa | https://github.com/saykowa/SandboxieAPI

#pragma warning disable CA1416

/// <summary> 
/// Main Sandboxie class with static functions in it. 
/// </summary>
public abstract class SandboxieWrapper
{
    /// <summary>
    /// Name of the Sandboxie path.
    /// </summary>
    private const string SbiePathName = "Sandboxie-Plus";

    /// <summary>
    /// Sandboxie's path.
    /// </summary>
    private static readonly string SbiePath = @$"{SettingsManager.CurrentSettings.General.SandboxiePath}";

    /// <summary>
    /// String URL with Sandboxie releases.
    /// </summary>
    private const string SbieReleases = "https://api.github.com/repos/sandboxie-plus/Sandboxie/releases";

    /// <summary> 
    /// Structure of required sandboxie file. 
    /// </summary>
    private struct SbieFile
    {
        /// <summary>
        /// Constructor of <see cref="SbieFile"/> with <paramref name="name"/> and <paramref name="extension"/> of the required file
        /// </summary>
        public SbieFile(string name, string extension)
        {
            Name = name;
            Extension = extension;
            Full = $"{Name}.{Extension}";
            Path = $@"{SbiePath}\{Full}";
        }

        /// <summary>
        /// Name of the file with extension.
        /// </summary>
        public string Full { get; set; }

        /// <summary>
        /// Name of the file without extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// File path.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary> 
    /// Sandboxie DLL file with native functions which will be used. 
    /// </summary>
    private static readonly SbieFile SbieDll = new("SbieDll", "dll");

    /// <summary> 
    /// Sandboxie Service execution file. 
    /// </summary>
    private static readonly SbieFile SbieService = new("SbieSvc", "exe");

    /// <summary> 
    /// Sandboxie Message DLL file which contains messages from other native sandboxie functions. 
    /// </summary>
    private static readonly SbieFile SbieMessage = new("SbieMsg", "dll");

    /// <summary> 
    /// Sandboxie Driver SYS file which used to manage commands between sandboxie and service. 
    /// </summary>
    private static readonly SbieFile SbieDriver = new("SbieDrv", "sys");

    /// <summary> 
    /// Sandboxie KmdUtil execution file which used to installing or removing sandboxie's components. 
    /// </summary>
    private static readonly SbieFile SbieKmdUtil = new("KmdUtil", "exe");

    /// <summary> 
    /// Sandboxie statuses which helps to easily debug the code and errors. 
    /// </summary>
    public enum SB_STATUS
    {
        SB_OK,
        SB_PATH_NOT_EXISTS,
        SB_NOT_EXISTS,
        SB_NO_ACCESS,
        SB_IS64BIT_NOT_64,
        SB_IS64BIT_ERROR,
        SB_EXECUTE_ERROR,
        SB_EXECUTE_CAN_NOT_EXECUTE,
        SB_EXECUTE_NOT_FOUND,
        SB_KMDUTIL_EXEC_ERROR,
        SB_KMDUTIL_INSTALL_SERVICE_ERROR,
        SB_KMDUTIL_INSTALL_SERVICE_NOT_EXISTS,
        SB_KMDUTIL_INSTALL_DRIVER_ERROR,
        SB_KMDUTIL_INSTALL_DRIVER_NOT_EXISTS,
        SB_KMDUTIL_STOP_SERVICE_ERROR,
        SB_KMDUTIL_STOP_SERVICE_NOT_EXISTS,
        SB_KMDUTIL_STOP_SERVICE_EXEC_ERROR,
        SB_KMDUTIL_REMOVE_SERVICE_NOT_EXISTS,
        SB_KMDUTIL_REMOVE_DRIVER_NOT_EXISTS,
        SB_KMDUTIL_REMOVE_DRIVER_ERROR,
        SB_WEB_BAD_STATUS_CODE,
        SB_WEB_NOT_FOUND,
        SB_WEB_NO_RELEASES,
        SB_WEB_NO_ASSETS,
        SB_WEB_NO_REQUIED_ASSET,
        SB_WEB_NO_DOWNLOAD_URL,
        SB_DOWNLOAD_TIMEOUTED,
        SB_DOWNLOAD_NO_URL,
        SB_DOWNLOAD_FAILED,
        SB_DOWNLOAD_FILE_NOT_FOUND,
        SB_INSTALL_WEB_PARSING_ERROR,
        SB_INSTALL_DOWNLOAD_ERROR,
        SB_INSTALL_UNPACKING_ERROR,
        SB_INSTALL_FILE_NOT_FOUND,
        SB_INSTALL_COMPONENTS_ERROR,
        SB_REMOVE_SERVICE_ERROR,
        SB_REMOVE_SERVICE_NOT_STOPPED,
        SB_REMOVE_DRIVER_ERROR,
        SB_REMOVE_COMPONENTS_ERROR,
        SB_REMOVE_PATH_NOT_EXISTS,
        SB_SERVICE_START_NOT_EXISTS,
        SB_SERVICE_START_DLL_ERROR,
        SB_CONFIG_EDIT_NOT_EXISTS,
        SB_CONFIG_EDIT_DLL_ERROR,
        SB_CONFIG_RELOAD_NOT_EXISTS,
        SB_CONFIG_RELOAD_DLL_ERROR,
        SB_GET_SBIE_HOME_PATH_DLL_ERROR,
        SB_GET_CONFIG_VALUE_ERROR,
        SB_GET_BOXED_PIDS_ERROR,
        SB_GET_BOX_PATH_ERROR,
        SB_BOX_NO_ANY_CONFIGURATION,
        SB_BOX_IS_ACTIVE_ERROR,
        SB_BOX_LOAD_DEFAULTS_ERROR,
        SB_BOX_LOAD_DEFAULTS_BOX_NOT_EXISTS,
        SB_BOX_REMOVE_ERROR,
        SB_BOX_REMOVE_BOX_IS_ACTIVE,
        SB_RUN_BOXED_NOT_EXISTS,
        SB_RUN_BOXED_ERROR,
        SB_UNKNOWN,
        SB_WRONG_DIR,
        SB_SERVICE_NOT_FOUND,
        SB_SERVICE_NOT_STARTED,
        SB_DRIVER_NOT_FOUND,
    }

    /// <summary> 
    /// Class with the <see cref="SB_STATUS"/> and the <see cref="string"/>. 
    /// Used to collect statuses and errors strings from functions. 
    /// </summary>
    public class SB_ERROR
    {
        /// <summary> 
        /// Value with type of <see cref="SB_STATUS"/> which means an error status. 
        /// </summary>
        public SB_STATUS Status { get; }

        /// <summary> 
        /// Error message of the operation. 
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary> 
        /// <see cref="SB_ERROR"/> class constructor. 
        /// </summary>
        /// <param name="status"> Status code of the occurred error with the <see cref="SB_STATUS"/> type of value </param>
        /// <param name="errorMsg"> String message of the occurred error </param>
        public SB_ERROR(SB_STATUS status, string errorMsg)
        {
            Status = status;
            ErrorMessage = errorMsg;
        }
    }

    /// <summary> 
    /// Results class with operation's status, result, data and errors list. Used as a return value from functions.
    /// For debugging errors (if <see cref="Result"/> is False) you can use <see cref="ErrorsList"/>
    /// </summary>
    public class SB_RESULT<T>
    {
        /// <summary> 
        /// Value with type of <see cref="SB_STATUS"/> which means a status of executed function. 
        /// </summary>
        public SB_STATUS Status { get; }

        /// <summary> 
        /// Value with type of <see cref="bool"/>. True when operation success or False when any errors occured. 
        /// </summary>
        public bool Result { get; }

        /// <summary> 
        /// Value with unsettled <see cref="Type"/>. Type of <typeparamref name="T"/> determines the type of collected result data. 
        /// </summary>
        public T? Data { get; }

        /// <summary> 
        /// List of <see cref="SB_ERROR"/> values. Used to collect multiple errors from another functions to make a trace-view of occured error. 
        /// </summary>
        public List<SB_ERROR> ErrorsList { get; }

        /// <summary> 
        /// <see cref="SB_RESULT{T}"/> class constructor. 
        /// </summary>
        /// <param name="operationStatus"> Status of operation. If no any errors always should be as <see cref="SB_STATUS.SB_OK"/> </param>
        /// <param name="operationResult"> <see cref="bool"/> value which is True if operation have not any errors or False if it does. </param>
        /// <param name="operationData"> The value of completed operation. Value type must be set on new class object creation. </param>
        /// <param name="operationErrMsg"> 
        /// Error message of operation. If the message was described in the code,
        /// it will be automatically added to the status in the <see cref="ErrorsList"/>, 
        /// if not, the standard error message "Operation failed" will appear instead.
        /// </param>
        public SB_RESULT(SB_STATUS operationStatus, bool operationResult, T? operationData = default, string? operationErrMsg = null)
        {
            Status = operationStatus;
            Result = operationResult;
            Data = operationData;
            ErrorsList = operationResult ? new List<SB_ERROR>() : new List<SB_ERROR> { new(operationStatus, operationErrMsg ?? "Operation failed") };
        }

        /// <summary> 
        /// Adds a single error to <see cref="ErrorsList"/>. 
        /// </summary>
        /// <param name="status"> Operation status for debugging errors. Uses available status enums from <see cref="SB_STATUS"/> </param>
        /// <param name="errMsg"> Message of appeared error </param>
        public void AddErrorStatus(SB_STATUS status, string errMsg) => ErrorsList.Add(new SB_ERROR(status, errMsg));

        /// <summary> 
        /// Adds multiple errors to <see cref="ErrorsList"/>. 
        /// </summary>
        /// <param name="sbErrors"> List of <see cref="SB_ERROR"/> objects </param>
        public void AddErrorsStatuses(List<SB_ERROR> sbErrors) => sbErrors.ForEach(ErrorsList.Add);
    }

    /// <summary>
    /// Checks if there's <see cref="SbiePath"/> exists.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> IsSbieExists()
    {
        try
        {
            var sbieExists = Directory.GetFiles(SbiePath).Any(file => Path.GetFileName(file) == "SandMan.exe");

            return sbieExists ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, sbieExists, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_NOT_EXISTS, sbieExists, false);
        }

        catch (Exception err)
        {
            return err switch
            {
                UnauthorizedAccessException => new SB_RESULT<bool>(SB_STATUS.SB_NO_ACCESS, false, false, err.Message),
                DirectoryNotFoundException => new SB_RESULT<bool>(SB_STATUS.SB_PATH_NOT_EXISTS, false, false, err.Message),
                _ => new SB_RESULT<bool>(SB_STATUS.SB_UNKNOWN, false, false, err.Message)
            };
        }
    }

    /// <summary>
    /// Checks system's drivers for <see cref="SbieDriver"/> existing.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> IsDriverExists()
    {
        try
        {
            using var serviceDriverKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{SbieDriver.Name}");

            return serviceDriverKey != null ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_DRIVER_NOT_FOUND, false, false);
        }

        catch (Exception err)
        {
            return err is SecurityException ?
                new SB_RESULT<bool>(SB_STATUS.SB_NO_ACCESS, false, false, err.Message) :
                new SB_RESULT<bool>(SB_STATUS.SB_UNKNOWN, false, false);
        }
    }

    /// <summary>
    /// Checks system's services for <see cref="SbieService"/> existing.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> IsServiceExists()
    {
        ServiceController? sbieService;
        try
        {
            sbieService = new ServiceController(SbieService.Name);
            _ = sbieService.Status;
        }

        catch (Exception err)
        {
            return new SB_RESULT<bool>(SB_STATUS.SB_SERVICE_NOT_FOUND, false, false, err.Message);
        }

        return sbieService.Status is ServiceControllerStatus.Running ?
            new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
            new SB_RESULT<bool>(SB_STATUS.SB_SERVICE_NOT_STARTED, false, false);
    }

    /// <summary>
    /// Tries to get latest asset download URL from <see cref="SbieReleases"/>
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="string"/> (URL of asset) on success 
    /// or <see cref="Nullable"/> on error. 
    /// </returns>
    public static SB_RESULT<string?> GetLastAssetUrl()
    {
        using HttpClient hClient = new();
        hClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36 Edg/117.0.2045.60");
        hClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = hClient.GetAsync(SbieReleases).GetAwaiter().GetResult();
        if ((int)response.StatusCode != 200)
            return new SB_RESULT<string?>(SB_STATUS.SB_WEB_BAD_STATUS_CODE, false, null, "Web request failed with status code " + response.StatusCode);

        var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var responseJson = JArray.Parse(responseBody);
        var release = responseJson[0];
        var releaseAssets = release["assets"];
        if (releaseAssets is null)
            return new SB_RESULT<string?>(SB_STATUS.SB_WEB_NO_RELEASES, false, null, "No release assets found");

        var requiredAsset = releaseAssets.FirstOrDefault(el =>
        {
            var requiredAssetName = (string?)el["name"];
            return requiredAssetName != null && requiredAssetName.Contains("x64") && requiredAssetName.Contains("Plus");
        });
        if (requiredAsset is null)
            return new SB_RESULT<string?>(SB_STATUS.SB_WEB_NO_REQUIED_ASSET, false, null, "Required asset not found");

        var downloadUrl = (string?)requiredAsset["browser_download_url"];
        return downloadUrl is null ?
            new SB_RESULT<string?>(SB_STATUS.SB_WEB_NO_DOWNLOAD_URL, false, null, "Download url not found") :
            new SB_RESULT<string?>(SB_STATUS.SB_OK, true, downloadUrl);
    }

    /// <summary>
    /// Tries to download asset from <see cref="GetLastAssetUrl"/> function.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="string"/> (Asset Name) 
    /// or <see cref="Nullable"/> on error. 
    /// </returns>
    public static SB_RESULT<string?> DownloadAsset(bool retry = false)
    {
        var assetUrlResult = GetLastAssetUrl();
        var assetUrl = assetUrlResult.Data;

        if (assetUrl is null)
        {
            var downloadAssetResult = new SB_RESULT<string?>(SB_STATUS.SB_DOWNLOAD_NO_URL, false, null, "Download URL for asset wasn't parsed");
            downloadAssetResult.AddErrorsStatuses(assetUrlResult.ErrorsList);
            return downloadAssetResult;
        }

        var assetName = Path.GetFileName(new Uri(assetUrl).LocalPath);

        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var currentAsset = Directory.GetFiles(currentDir).FirstOrDefault(el => el.Contains(assetName) || (el.Contains("Plus") && el.Contains("x64")));
        if (currentAsset is not null) return new SB_RESULT<string?>(SB_STATUS.SB_OK, true, Path.GetFileName(currentAsset));

        var savePath = $@"{currentDir}/{assetName}";
        var retries = retry ? 3 : 1;

        for (var indexer = 0; indexer < retries; indexer++)
        {
            try
            {
                using HttpClient hClient = new();
                hClient.Timeout = TimeSpan.FromSeconds(20);

                var fileBytes = hClient.GetByteArrayAsync(assetUrl).GetAwaiter().GetResult();
                if (fileBytes.Length is 0)
                    return new SB_RESULT<string?>(SB_STATUS.SB_DOWNLOAD_FAILED, false, null, $"Failed to download the asset {(retry ? $"after {retries} retries" : "")}");

                File.WriteAllBytes(savePath, fileBytes);
                break;
            }
            catch
            {
                if (indexer + 1 == retries)
                    return new SB_RESULT<string?>(SB_STATUS.SB_DOWNLOAD_TIMEOUTED, false, null, $"Request was timeout {(retry ? $"in all {retries} retries" : "")}");
            }
        }

        var assetFile = Directory.GetFiles(currentDir).FirstOrDefault(el => el.Contains(assetName));

        return assetFile is null ?
            new SB_RESULT<string?>(SB_STATUS.SB_DOWNLOAD_FILE_NOT_FOUND, false, null, "Failed to find the downloaded asset") :
            new SB_RESULT<string?>(SB_STATUS.SB_OK, true, assetName);
    }

    /// <summary>
    /// Trying to install sandboxie and required components.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> Install()
    {
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var sandboxieDir = Directory.GetDirectories(currentDir).FirstOrDefault(el =>
        {
            var elPath = Path.GetDirectoryName(el);
            return elPath is not null && elPath.Contains("Sandboxie-Plus");
        });

        if (sandboxieDir is not null)
        {
            var installSbieDriverResult = KmdUtil.InstallSbieDriver();
            var installSbieServiceResult = KmdUtil.InstallSbieService();

            return installSbieDriverResult.Result && installSbieServiceResult.Result
                ? new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true)
                : new SB_RESULT<bool>(SB_STATUS.SB_UNKNOWN, false, false);
        }

        var downloaded = DownloadAsset();

        if (!downloaded.Result)
        {
            var installResult = new SB_RESULT<bool>(SB_STATUS.SB_INSTALL_DOWNLOAD_ERROR, false, false, "Installing was incomplete due to download error");
            installResult.AddErrorsStatuses(downloaded.ErrorsList);
            return installResult;
        }

        Process sbieInstaller = new()
        {
            StartInfo =
            {
                FileName = @$"{currentDir}/{downloaded.Data}",
                Arguments = "/VERYSILENT /PORTABLE=1",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            }
        };

        try
        {
            sbieInstaller.Start();
            sbieInstaller.WaitForExit();
        }

        catch
        {
            return new SB_RESULT<bool>(SB_STATUS.SB_INSTALL_UNPACKING_ERROR, false, false,
                "Failed to start installer or it is was closed/crashed during process");
        }

        sandboxieDir = Directory.GetDirectories(currentDir).FirstOrDefault(el => el.Contains("Sandboxie-Plus"));

        if (sandboxieDir is null)
            return new SB_RESULT<bool>(SB_STATUS.SB_INSTALL_FILE_NOT_FOUND, false, false, "Failed to find Sandboxie directory after installing");

        File.Delete(@$"{currentDir}/{downloaded.Data}");
        File.Delete(@$"{sandboxieDir}/SbieCtrl.exe");

        var installSbieDrvStatus = KmdUtil.InstallSbieDriver();
        var installSbieSvcStatus = KmdUtil.InstallSbieService();
        var startSbieSvcStatus = StartSbieService();
        var loadGlobalDefaultsStatus = Config.LoadGlobalDefaults();

        if (installSbieDrvStatus.Result && installSbieSvcStatus.Result && startSbieSvcStatus.Result && loadGlobalDefaultsStatus.Result)
            return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

        var installStatus = new SB_RESULT<bool>(SB_STATUS.SB_INSTALL_COMPONENTS_ERROR, false, false, "Failed to install Sandboxie components");
        installStatus.AddErrorsStatuses(installSbieDrvStatus.ErrorsList.Concat(installSbieSvcStatus.ErrorsList)
            .Concat(startSbieSvcStatus.ErrorsList)
            .Concat(loadGlobalDefaultsStatus.ErrorsList)
            .ToList());
        return installStatus;
    }

    /// <summary>
    /// Trying to remove sandboxie and components.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> Remove()
    {
        if (!Directory.Exists(SbiePath)) return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

        var sbieSvcRem = KmdUtil.RemoveSbieService();
        var sbieDrvRem = KmdUtil.RemoveSbieDriver();
        bool sbieRem = false;

        try
        {
            Directory.Delete(SbiePath, true);
            if (!Directory.Exists(SbiePath)) sbieRem = true;
        }

        catch (Exception err)
        {
            return err switch
            {
                UnauthorizedAccessException => new SB_RESULT<bool>(SB_STATUS.SB_NO_ACCESS, false, false, "Can't remove paths without administrator privileges"),
                DirectoryNotFoundException => new SB_RESULT<bool>(SB_STATUS.SB_PATH_NOT_EXISTS, false, false, "Can't remove non-exists path"),
                _ => new SB_RESULT<bool>(SB_STATUS.SB_UNKNOWN, false, false, err.Message)
            };
        }

        if (sbieSvcRem.Result && sbieDrvRem.Result && sbieRem)
            return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

        return new SB_RESULT<bool>(SB_STATUS.SB_REMOVE_COMPONENTS_ERROR, false, false, "Failed to remove Sandboxie components");
    }

    /// <summary> 
    /// Starts Sandboxie service.
    /// </summary>
    /// <param name="retry"> Enabling or disabling retries to start service. </param>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> StartSbieService(bool retry = false)
    {
        var sbieExists = IsSbieExists();
        var serviceExists = IsServiceExists();

        if (!sbieExists.Result)
        {
            var startSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_SERVICE_START_NOT_EXISTS, false, false, "Failed to start service because sandboxie path is not exists");
            startSbieServiceResult.AddErrorsStatuses(sbieExists.ErrorsList);
            return startSbieServiceResult;
        }

        if (!serviceExists.Result && serviceExists.Status != SB_STATUS.SB_SERVICE_NOT_STARTED)
        {
            var startSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_SERVICE_START_DLL_ERROR, false, false, "Failed to start service because sandboxie service is not exists");
            startSbieServiceResult.AddErrorsStatuses(serviceExists.ErrorsList);
            return startSbieServiceResult;
        }

        var nativeResult = Api.StartSbieSvc(retry);
        var startSbieSvc = Convert.ToBoolean(nativeResult);

        return startSbieSvc ?
            new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
            new SB_RESULT<bool>(SB_STATUS.SB_SERVICE_START_DLL_ERROR, false, false, "Failed to start service because SbieDll operation was not completed");
    }

    /// <summary> 
    /// Gets Sandboxie installation path.
    /// </summary>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="string"/>. 
    /// Value is exists on success or <see cref="Nullable"/> on error. 
    /// </returns>
    public static SB_RESULT<string?> GetHomePath()
    {
        uint bufferLength = 248;
        string ntPath = new(' ', (int)bufferLength);
        string dosPath = new(' ', (int)bufferLength);

        var result = Convert.ToBoolean(Api.GetHomePath(ntPath, bufferLength, dosPath, bufferLength));

        return result is false ?
            new SB_RESULT<string?>(SB_STATUS.SB_GET_SBIE_HOME_PATH_DLL_ERROR, false, null, "Can't get sandboxie installation path") :
            new SB_RESULT<string?>(SB_STATUS.SB_OK, true, ntPath);
    }

    /// <summary>
    /// Checks file for executing attributes.
    /// </summary>
    /// <param name="filePath"> Path to file </param>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error. 
    /// </returns>
    public static SB_RESULT<bool> CanWinExecute(string filePath)
    {
        try
        {
            if (Path.IsPathRooted(filePath))
            {
                if (!File.Exists(filePath))
                    return new SB_RESULT<bool>(SB_STATUS.SB_EXECUTE_NOT_FOUND, false, false, "File was not found");

                var fileAttributes = File.GetAttributes(filePath);
                if ((fileAttributes & FileAttributes.ReparsePoint) == 0)
                    return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);
            }
            else
            {
                var environmentWinPath = Environment.GetEnvironmentVariable("PATH");
                if (environmentWinPath is null)
                    return new SB_RESULT<bool>(SB_STATUS.SB_EXECUTE_NOT_FOUND, false, false, "File was not found");

                var winPaths = environmentWinPath.Split(Path.PathSeparator);

                foreach (var winPath in winPaths)
                {
                    var fullPath = Path.Combine(winPath, filePath);
                    if (!File.Exists(fullPath)) continue;

                    var fileAttributes = File.GetAttributes(fullPath);
                    return (fileAttributes & FileAttributes.ReparsePoint) == 0 ?
                        new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                        new SB_RESULT<bool>(SB_STATUS.SB_EXECUTE_CAN_NOT_EXECUTE, false, false, "File was found but has no \"Executable\" attribute");
                }
            }

            return new SB_RESULT<bool>(SB_STATUS.SB_EXECUTE_NOT_FOUND, false, false, "File was not found");
        }
        catch (Exception err)
        {
            return new SB_RESULT<bool>(SB_STATUS.SB_EXECUTE_ERROR, false, false, $"Exception was caught during function process: {err.Message}");
        }
    }

    /// <summary>
    /// Checks file for 64-bit flag.
    /// </summary>
    /// <param name="filePath"> Path to file </param>
    /// <returns> 
    /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
    /// Value is True on success or False on error.
    /// </returns>
    public static SB_RESULT<bool> Is64Bit(string filePath)
    {
        try
        {
            var fileAssembly = Assembly.LoadFile(Path.GetFullPath(filePath));
            var fileModule = fileAssembly.GetModules()[0];

            fileModule.GetPEKind(out var peKind, out _);

            var has64BitFlag = peKind.HasFlag(PortableExecutableKinds.PE32Plus);
            return has64BitFlag ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_IS64BIT_NOT_64, false, false, "File is not 64-bit executable");
        }
        catch (Exception err)
        {
            return new SB_RESULT<bool>(SB_STATUS.SB_IS64BIT_ERROR, false, false, $"Exception was occurred: {err.Message}");
        }
    }

    public static SB_RESULT<Process?> RunBoxed(string filePath, string boxName = "DefaultBox")
    {
        if (!IsSbieExists().Result)
            return new SB_RESULT<Process?>(SB_STATUS.SB_RUN_BOXED_ERROR, false, null, "Boxed launching was not completed because sandboxie path is not exists");
        if (!File.Exists(filePath))
            return new SB_RESULT<Process?>(SB_STATUS.SB_RUN_BOXED_NOT_EXISTS, false, null, "Can not launch process which is not exists");

        Api.STARTUPINFO startupInfo = new();
        startupInfo.cb = Marshal.SizeOf(startupInfo);

        var startResult = Api.RunSandboxed(boxName, filePath, SbiePath, 0, ref startupInfo, out var processInfo);

        return startResult ?
            new SB_RESULT<Process?>(SB_STATUS.SB_OK, true, Process.GetProcessById((int)processInfo.dwProcessId)) :
            new SB_RESULT<Process?>(SB_STATUS.SB_RUN_BOXED_ERROR, false, null, "Failed to start sandboxed process");
    }

    public static SB_RESULT<List<Process>?> GetBoxedProcesses(string boxName = "DefaultBox")
    {
        var processes = new List<Process>();
        uint processesBuffer = 1024;
        uint boxedCounter = 0;

        unchecked
        {
            var enumProcessExResult = Api.EnumProcessEx(boxName, false, (uint)-1, null, ref boxedCounter);
            if (enumProcessExResult != 0)
                return new SB_RESULT<List<Process>?>(SB_STATUS.SB_GET_BOXED_PIDS_ERROR, false, null, $"Error occurred while attempting to get PIDs for {boxName} box");

            boxedCounter += processesBuffer;
            var boxedPiDs = new uint[boxedCounter];

            enumProcessExResult = Api.EnumProcessEx(boxName, false, (uint)-1, boxedPiDs, ref boxedCounter);
            boxedPiDs = boxedPiDs.Where(el => el != 0).ToArray();

            if (enumProcessExResult != 0)
                return new SB_RESULT<List<Process>?>(SB_STATUS.SB_GET_BOXED_PIDS_ERROR, false, null, $"Error occurred while attempting to get PIDs for {boxName} box");
            if (boxedCounter == 0)
                return new SB_RESULT<List<Process>?>(SB_STATUS.SB_OK, true, null);

            processes.AddRange(boxedPiDs.Select(boxedPid => Process.GetProcessById((int)boxedPid)));

            return new SB_RESULT<List<Process>?>(SB_STATUS.SB_OK, true, processes);
        }
    }

    public static SB_RESULT<bool> RemoveBox(string boxName)
    {
        var box = new Box(boxName);

        if (box.Active)
            return new SB_RESULT<bool>(SB_STATUS.SB_BOX_REMOVE_BOX_IS_ACTIVE, false, false, $"Cant remove box with active status");

        var deleteConfig = Config.Edit(Config.Operations.Overwrite, boxName, Config.Keys.Asterisk, "");
        return deleteConfig.Result is false
            ? new SB_RESULT<bool>(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while deleting old box configuration")
            : new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);
    }

    public static SB_RESULT<bool> IsBoxActive(string boxName)
    {
        var boxedProcesses = GetBoxedProcesses(boxName);
        return boxedProcesses.Result switch
        {
            false => new SB_RESULT<bool>(SB_STATUS.SB_BOX_IS_ACTIVE_ERROR, false, false,
                $"Failed to get processes for {boxName}"),
            true when boxedProcesses.Data is null => new SB_RESULT<bool>(SB_STATUS.SB_OK, true, false),
            _ => new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true)
        };
    }

    public static SB_RESULT<string?> GetBoxPath(string boxName)
    {
        uint targetPathLength = 260;
        StringBuilder targetPath = new((int)targetPathLength);

        uint sbieBuffetLength = 1024;
        StringBuilder sbieFilePath = new((int)sbieBuffetLength);
        StringBuilder sbieKeyPath = new((int)sbieBuffetLength);
        StringBuilder sbieIpcPath = new((int)sbieBuffetLength);

        if (Api.QueryBoxPath(boxName, sbieFilePath, sbieKeyPath, sbieIpcPath, ref sbieBuffetLength, ref sbieBuffetLength, ref sbieBuffetLength) != 0)
            return new SB_RESULT<string?>(SB_STATUS.SB_GET_BOX_PATH_ERROR, false, null, "Failed to execute SbieDll_QueryBoxPath");

        var filePath = sbieFilePath.ToString();
        var keyPath = sbieKeyPath.ToString();
        var ipcPath = sbieIpcPath.ToString();

        var splitFilePath = filePath.Split("\\");
        var targetDevicePath = @$"\{splitFilePath[1]}\{splitFilePath[2]}";

        var filePathDrive = DriveInfo.GetDrives().First(drive =>
        {
            if (!drive.IsReady) return false;

            StringBuilder devicePath = new((int)targetPathLength);
            var isQddExecuted = Api.QueryDosDevice(drive.Name.TrimEnd('\\'), devicePath, targetPathLength) != 0;
            return isQddExecuted && devicePath.ToString() == targetDevicePath;
        });

        targetPath.Append(filePathDrive.Name + filePath.Replace(targetDevicePath, "").Remove(0, 1));
        return new SB_RESULT<string?>(SB_STATUS.SB_OK, true, targetPath.ToString());

    }

    /// <summary> 
    /// Class with functions which using <see cref="SbieKmdUtil"/> for commands executing. 
    /// </summary>
    public class KmdUtil
    {
        /// <summary>
        ///  Execution function for <see cref="SbieKmdUtil"/>
        /// </summary>
        /// <param name="arguments"> 
        /// Arguments presented as a string 
        /// <example><code>command name file_path type=? start=? msgfile=msg_file_path</code></example> 
        /// </param>
        /// <returns> Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. </returns>
        public static SB_RESULT<bool> Exec(string arguments = "")
        {
            Process pKmdUtil = new()
            {
                StartInfo =
                {
                    FileName = SbieKmdUtil.Path,
                    Arguments = arguments,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                }
            };

            try
            {
                pKmdUtil.Start();
                pKmdUtil.WaitForExit();
                return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);
            }

            catch (Exception err)
            {
                return new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_EXEC_ERROR, false, false, err.Message);
            }
        }

        /// <summary>
        /// Tries to install <see cref="SbieService"/>.
        /// </summary>
        /// <param name="Retry"> Enables or disables installing retries (disabled by default). </param>
        /// <returns> Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. </returns>
        public static SB_RESULT<bool> InstallSbieService()
        {
            var isSbieExists = IsSbieExists();

            if (!isSbieExists.Result)
            {
                var installSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_INSTALL_SERVICE_ERROR, false, false, "Failed to install service because sandboxie path is not exists");
                installSbieServiceResult.AddErrorsStatuses(isSbieExists.ErrorsList);
                return installSbieServiceResult;
            }

            if (IsServiceExists().Result)
                return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            var execResult = Exec($"install {SbieService.Name} {AppDomain.CurrentDomain.BaseDirectory}{SbiePathName}\\{SbieService.Full} type=own start=auto \"display=Sandboxie Service\" group=UIGroup \"msgfile={SbieMessage.Path}\"");
            var startSbieService = StartSbieService();
            var sbieServiceExists = IsServiceExists();
            if (execResult.Result && startSbieService.Result && sbieServiceExists.Result)
                return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            var sbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_UNKNOWN, false, false,
                "Failed to install sandboxie service");
            sbieServiceResult.AddErrorsStatuses(execResult.ErrorsList.Concat(execResult.ErrorsList)
                .ToList());
            return sbieServiceResult;
        }

        /// <summary>
        /// Tries to remove <see cref="SbieService"/>.
        /// </summary>
        /// <returns> Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. </returns>
        public static SB_RESULT<bool> RemoveSbieService()
        {
            var sbiePathExists = IsSbieExists();
            var serviceExists = IsServiceExists();

            if (!sbiePathExists.Result)
            {
                var removeSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_REMOVE_PATH_NOT_EXISTS, false, false, "Error while removing service because sandboxie path not found or not exists");
                removeSbieServiceResult.AddErrorsStatuses(sbiePathExists.ErrorsList);
                return removeSbieServiceResult;
            }
            ;

            if (!serviceExists.Result) return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            //var stopSbieService = StopSbieService();
            //if (!stopSbieService.Result)
            //{
                //var removeSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_REMOVE_SERVICE_ERROR, false, false, "Error while removing service because it can't be stopped");
                //removeSbieServiceResult.AddErrorsStatuses(stopSbieService.ErrorsList);
                //return removeSbieServiceResult;
            //}
            //;

            var deleteResult = Exec($"delete {SbieService.Name}");

            if (deleteResult.Result is false)
            {
                var removeSbieServiceResult = new SB_RESULT<bool>(SB_STATUS.SB_REMOVE_SERVICE_ERROR, false, false, "Error while removing service because executing command was not completed");
                removeSbieServiceResult.AddErrorsStatuses(deleteResult.ErrorsList);
                return removeSbieServiceResult;
            }

            serviceExists = IsServiceExists();

            return !serviceExists.Result ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_REMOVE_SERVICE_ERROR, false, false, "Error while removing service because of unknown error");
        }


        /// <summary>
        /// Tries to install <see cref="SbieDriver"/>.
        /// </summary>
        /// <param name="Retry"> Enables or disables driver installing retries (disabled by default). </param>
        /// <returns> Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. </returns>
        public static SB_RESULT<bool> InstallSbieDriver(bool Retry = false)
        {
            var isSbieExists = IsSbieExists();
            var isDriverExists = IsDriverExists();

            SB_RESULT<bool>? installSbieDriverResult;

            if (!isSbieExists.Result)
            {
                installSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_INSTALL_DRIVER_NOT_EXISTS, false, false, "Error while installing driver because sandboxie path is not exists");
                installSbieDriverResult.AddErrorsStatuses(isSbieExists.ErrorsList);
                return installSbieDriverResult;
            }

            if (!isDriverExists.Result)
            {
                var install = Exec($"install {SbieDriver.Name} {AppDomain.CurrentDomain.BaseDirectory}{SbiePathName}\\{SbieDriver.Full} type=kernel start=demand \"msgfile={SbieMessage.Path}\" altitude=\"86900\"");
                if (!install.Result)
                {
                    installSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_INSTALL_DRIVER_ERROR, false, false, "Error while installing driver because execution command was not completed");
                    installSbieDriverResult.AddErrorsStatuses(isDriverExists.ErrorsList);
                    return installSbieDriverResult;
                }
            }

            else return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            isDriverExists = IsDriverExists();
            if (isDriverExists.Result)
                return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            installSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_INSTALL_DRIVER_ERROR, false, false, "Error while installing driver because it was not found after success installing");
            installSbieDriverResult.AddErrorsStatuses(isDriverExists.ErrorsList);
            return installSbieDriverResult;
        }

        /// <summary>
        /// Tries to remove <see cref="SbieDriver"/>.
        /// </summary>
        /// <returns> Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. </returns>
        public static SB_RESULT<bool> RemoveSbieDriver()
        {
            var isSbieExists = IsSbieExists();
            var isDriverExists = IsDriverExists();

            SB_RESULT<bool>? removeSbieDriverResult;

            if (!isSbieExists.Result)
            {
                removeSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_REMOVE_DRIVER_NOT_EXISTS, false, false, "Error while removing driver because sandboxie path is not exists");
                removeSbieDriverResult.AddErrorsStatuses(isSbieExists.ErrorsList);
                return removeSbieDriverResult;
            }

            if (isDriverExists.Result)
            {
                var stop = Exec($"stop {SbieDriver.Name}");
                var remove = Exec($"delete {SbieDriver.Name}");
                if (!remove.Result)
                {
                    removeSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_REMOVE_DRIVER_ERROR, false, false, "Error while removing driver because execution command was not completed");
                    removeSbieDriverResult.AddErrorsStatuses(isDriverExists.ErrorsList);
                    return removeSbieDriverResult;
                }
            }

            else return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            isDriverExists = IsDriverExists();

            if (!isDriverExists.Result)
                return new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true);

            removeSbieDriverResult = new SB_RESULT<bool>(SB_STATUS.SB_KMDUTIL_REMOVE_DRIVER_ERROR, false, false, "Error while removing driver because it is exists after operation success");
            removeSbieDriverResult.AddErrorsStatuses(isDriverExists.ErrorsList);
            return removeSbieDriverResult;
        }
    }

    /// <summary> 
    /// Class with functions that change Sandboxie configuration settings.
    /// </summary>
    public class Config
    {
        /// <summary> 
        /// Available keys of the Sandboxie configuration.
        /// </summary>
        public enum Keys
        {
            AlertFolder,
            AlertProcess,
            AutoDelete,
            AutoExec,
            AutoRecover,
            AutoRecoverIgnore,
            BlockNetParam,
            BlockNetworkFiles,
            BlockPassword,
            BorderColor,
            BoxNameTitle,
            BreakoutDocument,
            BreakoutFolder,
            BreakoutProcess,
            ClosedClsid,
            ClosedFilePath,
            ClosedIpcPath,
            ClosedKeyPath,
            ClosedRT,
            ConfigLevel,
            CopyLimitKb,
            CopyLimitSilent,
            DeleteCommand,
            Description,
            DisableRTBlacklist,
            DropAdminRights,
            EditAdminOnly,
            EditPassword,
            Enabled,
            FileRootPath,
            ForceDisableAdminOnly,
            ForceDisableSeconds,
            ForceFolder,
            ForceProcess,
            InjectDll,
            InjectDll64,
            IpcRootPath,
            KeyRootPath,
            LeaderProcess,
            LingerProcess,
            MonitorAdminOnly,
            NeverDelete,
            NoRenameWinClass,
            NormalFilePath,
            NormalIpcPath,
            NotifyDirectDiskAccess,
            NotifyInternetAccessDenied,
            NotifyProcessAccessDenied,
            NotifyStartRunAccessDenied,
            OpenClsid,
            OpenConfPath,
            OpenCredentials,
            OpenFilePath,
            OpenIpcPath,
            OpenKeyPath,
            OpenPipePath,
            OpenProtectedStorage,
            OpenSamEndpoint,
            OpenWinClass,
            ProcessLimit,
            ReadFilePath,
            ReadIpcPath,
            ReadKeyPath,
            RecoverFolder,
            Template,
            UseFileDeleteV2,
            UsePrivacyMode,
            UseRegDeleteV2,
            UseRuleSpecificity,
            UseSecurityMode,
            StartProgram,
            StartService,
            WriteFilePath,
            WriteKeyPath,
            Asterisk
        }

        /// <summary> 
        /// Enums of available operations for <see cref="Edit(Operations, string, Keys, string, string?)"/> function.
        /// </summary>
        public enum Operations
        {
            Overwrite = 's',
            Append = 'a',
            Insert = 'i',
            Delete = 'd'
        }

        public enum Options : uint
        {
            None = 0,
            DoNotScanGlobalSettings = 0x40000000,
            DoNotExpandVariables = 0x20000000,
            IgnoreTemplateSettings = 0x10000000
        }

        /// <summary> 
        /// Dictionary of keys and values of default sandbox.
        /// </summary>
        private static readonly Dictionary<Keys, List<string>> BoxDefaults = new()
        {
            { Keys.Enabled,           new() { "y" }                },
            { Keys.BlockNetworkFiles, new() { "y" }                },
            { Keys.RecoverFolder,     new() { "%Personal%",
                                                        "%Desktop%" }        },
            { Keys.BorderColor,       new() { "#00FFFF,ttl,2" }    },
            { Keys.Template,          new() { "AutoRecoverIgnore",
                                                         "LingerPrograms",
                                                         "BlockPorts",
                                                         "qWave",
                                                         "SkipHook",
                                                         "OpenBluetooth" }   },
            { Keys.ConfigLevel,       new() { "10" }               },
            { Keys.UseFileDeleteV2,   new() { "y" }                },
            { Keys.UseRegDeleteV2,    new() { "y" }                },
            { Keys.AutoRecover,       new() { "y" }                },
        };

        /// <summary> 
        /// Dictionary of keys and values of default global configuration.
        /// </summary>
        private static readonly Dictionary<Keys, List<string>> GlobalDefaults = new()
        {
            { Keys.FileRootPath, new() { $@"{SbiePath}\boxes\%SANDBOX%" } },
        };

        public static SB_RESULT<Dictionary<Keys, List<string>>> GetBoxDefaults() => new(SB_STATUS.SB_OK, true, BoxDefaults);

        public static SB_RESULT<Dictionary<Keys, List<string>>> GetGlobalDefaults() => new(SB_STATUS.SB_OK, true, GlobalDefaults);

        public static SB_RESULT<bool> LoadGlobalDefaults()
        {
            var globalDefaults = GetGlobalDefaults();
            var deleteConfig = Edit(Operations.Overwrite, "GlobalSettings", Keys.Asterisk, "");

            if (globalDefaults.Result is false || globalDefaults.Data is null)
                return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while getting default settings or value is null");

            if (deleteConfig.Result is false)
                return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while deleting old GlobalSettings configuration");

            var errorsOccurred = false;

            foreach (var valuePair in globalDefaults.Data)
            {
                valuePair.Value.ForEach(value =>
                {
                    var editResult = Edit(Operations.Append, "GlobalSettings", valuePair.Key, value);
                    if (editResult.Result is false) errorsOccurred = true;
                });
            }

            return errorsOccurred ?
                new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, "Errors occurred while configuration updating") :
                new(SB_STATUS.SB_OK, true, true);
        }

        /// <summary> 
        /// Modifies the configuration using the passed values as parameters.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/sandboxie-plus/sandboxie-docs/blob/main/Content/SBIEDLLAPI.md#update-configuration-in-sandboxieini">Sandboxie Github documentation</see>
        /// </remarks>
        /// <param name="operation"> 
        /// Type of edit operation. For readability of the code it is uses available enums from <see cref="Operations"/>.
        /// </param>
        /// <param name="section">
        /// Name of the section that will be changed (or added) as a result of the operation. 
        /// <code>
        /// [Section]
        /// Key=Value
        /// </code>
        /// </param>
        /// <param name="key">
        /// Name of the key that will be changed (or added) as a result of the operation. 
        /// <code>
        /// [Section]
        /// Key=Value
        /// </code>
        /// </param>
        /// <param name="value">
        /// Value that will be changed (or added) as a result of the operation.
        /// <code>
        /// [Section]
        /// Key=Value
        /// </code>
        /// </param>
        /// <param name="password"> The password of the Sandboxie configuration file (default is null) </param>
        /// <returns> 
        /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. Value is True on success or False on error. 
        /// </returns>
        public static SB_RESULT<bool> Edit(Operations operation, string section, Keys key, string value, string? password = null)
        {
            var isSbieExists = IsSbieExists();
            var keyString = key is Keys.Asterisk ? "*" : key.ToString();

            if (!isSbieExists.Result)
            {
                var editResult = new SB_RESULT<bool>(SB_STATUS.SB_CONFIG_EDIT_NOT_EXISTS, false, false, "Config editing was not completed because sandboxie path is not exists");
                editResult.AddErrorsStatuses(isSbieExists.ErrorsList);
                return editResult;
            }
            ;

            password ??= "";
            var result = !Convert.ToBoolean(Api.UpdateConf((char)operation, password, section, keyString, value));
            return result ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_CONFIG_EDIT_DLL_ERROR, false, result, "Config editing was not completed due to SbieDll bad return");
        }

        /// <summary>
        /// Gets values from configuration.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/sandboxie-plus/sandboxie-docs/blob/main/Content/SBIEDLLAPI.md#query-configuration-from-sandboxieini">Sandboxie Github documentation</see>
        /// </remarks>
        /// <param name="section">
        /// Specifies the section name that contains the setting key.
        /// </param>
        /// <param name="key">
        /// Key type which value will be caught.
        /// </param>
        /// <param name="keyIndex">
        /// Specifies the zero-based index number for a setting that may appear multiple times.
        /// <list>
        /// <term>
        /// NOTE
        /// </term>
        /// <description>
        /// The index number can be logically OR'ed with special values.
        /// </description>
        /// </list>
        /// </param>
        /// <param name="option">
        /// Operation option. Enum (list) of available options presented in <see cref="Options"/>
        /// </param>
        /// <returns> 
        /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="string"/>.
        /// Value is not null on success or null on error. 
        /// </returns>
        public static SB_RESULT<string?> Get(string section, Keys key, int keyIndex = 0, Options option = Options.None)
        {
            uint valueLength = 1024;
            StringBuilder valueBuffer = new((int)valueLength);

            var queryConfResult = Api.QueryConf(section, key.ToString(), option is Options.None ? (uint)keyIndex : (uint)option | (uint)keyIndex, valueBuffer, valueLength) == 0;
            return queryConfResult is false ?
                new SB_RESULT<string?>(SB_STATUS.SB_GET_CONFIG_VALUE_ERROR, false, null, "Error to get value from cfg due to QueryConf error") :
                new SB_RESULT<string?>(SB_STATUS.SB_OK, true, valueBuffer.ToString());
        }

        /// <summary>
        /// Gets multiple key values from configuration.
        /// </summary>
        /// <param name="section">
        /// Specifies the section name that contains the setting key.
        /// </param>
        /// <param name="key">
        /// Key type which value will be caught.
        /// </param>
        /// <param name="option">
        /// Operation option. Enum (list) of available options presented in <see cref="Options"/>
        /// </param>
        /// <returns> 
        /// Returns <see cref="SB_RESULT{T}"/> where type of data is a List of <see cref="string"/>.
        /// Value is not null on success or null on error. 
        /// </returns>
        public static SB_RESULT<List<string>?> GetAll(string section, Keys key, Options option = Options.None)
        {
            List<string> result = new();
            uint valueLength = 1024;
            StringBuilder valueBuffer = new((int)valueLength);

            uint keyIndex = 0;

            while (true)
            {
                var queryConfResult = Api.QueryConf(section, key.ToString(), option is Options.None ? keyIndex : (uint)option | (uint)Options.DoNotExpandVariables | keyIndex, valueBuffer, valueLength) == 0;
                if (queryConfResult is false || valueBuffer.Length <= 0) break;

                result.Add(valueBuffer.ToString());
                valueBuffer.Clear();
                keyIndex++;
            }

            return result.Count == 0 ?
                new SB_RESULT<List<string>?>(SB_STATUS.SB_GET_CONFIG_VALUE_ERROR, false, null, "Error to get value from cfg due to QueryConf error") :
                new SB_RESULT<List<string>?>(SB_STATUS.SB_OK, true, result);
        }

        /// <summary> 
        /// Reloads Sandboxie configuration file. Use this function after any applied changes.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/sandboxie-plus/sandboxie-docs/blob/main/Content/SBIEDLLAPI.md#reload-configuration-from-sandboxieini">Sandboxie Github documentation</see>
        /// </remarks>
        /// <param name="sessionId"> 
        /// Specifies the logon session number to which Sandboxie will log any error messages. 
        /// Default value is null which automatically sets SessionId as -1 (current logon session)
        /// </param>
        /// <returns> 
        /// Returns <see cref="SB_RESULT{T}"/> where type of data is <see cref="bool"/>. 
        /// Value is True on success or False on error. 
        /// </returns>
        public static SB_RESULT<bool> Reload(int? sessionId = null)
        {
            var isSbieExists = IsSbieExists();

            if (!isSbieExists.Result)
            {
                var reloadResult = new SB_RESULT<bool>(SB_STATUS.SB_CONFIG_EDIT_NOT_EXISTS, false, false, "Config editing was not completed because sandboxie path is not exists");
                reloadResult.AddErrorsStatuses(isSbieExists.ErrorsList);
                return reloadResult;
            }
            ;

            sessionId ??= -1;
            var result = !Convert.ToBoolean(Api.ReloadConf((uint)sessionId));
            return result ?
                new SB_RESULT<bool>(SB_STATUS.SB_OK, true, true) :
                new SB_RESULT<bool>(SB_STATUS.SB_CONFIG_RELOAD_DLL_ERROR, false, result, "Config reloading was not completed due to SbieDll bad return");
        }
    }

    /// <summary> 
    /// Box class with functions for the specified box.
    /// </summary>
    public class Box
    {
        /// <summary> 
        /// Name of the current box.
        /// </summary>
        public string Name { get; }

        /// <summary> 
        /// Current box existing status.
        /// </summary>
        public bool Exists
        {
            get
            {
                var enabledValue = Config.Get(Name, Config.Keys.Enabled);
                return enabledValue.Data is not null;
            }
        }

        /// <summary> 
        /// Current box enabled state in sandboxie configuration.
        /// </summary>
        public bool Enabled
        {
            get
            {
                var enabledValue = Config.Get(Name, Config.Keys.Enabled);
                return enabledValue.Data is not null && enabledValue.Data.StartsWith("y");
            }
        }

        /// <summary> 
        /// Current box active status.
        /// </summary>
        public bool Active => IsBoxActive(Name).Data;

        /// <summary> 
        /// List of <see cref="Process"/> objects which run in current box.
        /// </summary>
        public List<Process>? Processes => GetBoxedProcesses(Name).Data;

        /// <summary> 
        /// Configuration of current box.
        /// </summary>
        public Dictionary<Config.Keys, List<string>>? Configuration
        {
            get
            {
                var isConfigurationExists = Config.Get(Name, Config.Keys.Enabled).Result;
                if (isConfigurationExists is false) return null;

                var boxConfiguration = new Dictionary<Config.Keys, List<string>>();
                var availableKeys = Enum.GetValues(typeof(Config.Keys)).Cast<Config.Keys>().ToList();

                foreach (var availableKey in availableKeys)
                {
                    var setting = Config.GetAll(Name, availableKey, Config.Options.IgnoreTemplateSettings);
                    if (setting.Result is false || setting.Data is null) continue;
                    if (boxConfiguration.ContainsKey(availableKey)) boxConfiguration[availableKey].AddRange(setting.Data);
                    else boxConfiguration.Add(availableKey, setting.Data);
                }

                return boxConfiguration.Count > 0 ?
                    boxConfiguration :
                    null;

            }
        }

        /// <summary>
        /// Current box path (if exists).
        /// </summary>
        public string? Path => GetBoxPath(Name).Data;

        /// <summary> 
        /// <see cref="Box"/> class constructor.
        /// </summary>
        /// <param name="Name">
        /// The name of box which will be used.
        /// </param>
        public Box(string Name)
        {
            this.Name = Name;
        }

        public SB_RESULT<Process?> Run(string filePath)
        {
            return Configuration is null && Enabled is false ?
                new(SB_STATUS.SB_BOX_NO_ANY_CONFIGURATION, false, null, "Can't run in box because required configuration was not detected or box has false enabled param") :
                RunBoxed(filePath, Name);
        }

        public SB_RESULT<bool> Create()
        {
            if (Exists) return new SB_RESULT<bool>(SB_STATUS.SB_BOX_LOAD_DEFAULTS_BOX_NOT_EXISTS, false, false, "Can't create box that already exists. Use LoadDefaults() instead");

            var boxDefaults = Config.GetBoxDefaults();
            var deleteConfig = Config.Edit(Config.Operations.Overwrite, Name, Config.Keys.Asterisk, "");
            if (boxDefaults.Result is false || boxDefaults.Data is null) return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while getting default settings or value is null");
            if (deleteConfig.Result is false) return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while deleting old {Name} configuration");
            var errorsOccurred = false;

            foreach (var keyValuePair in boxDefaults.Data)
            {
                keyValuePair.Value.ForEach(value =>
                {
                    var editResult = Config.Edit(Config.Operations.Append, Name, keyValuePair.Key, value);
                    if (editResult.Result is false) errorsOccurred = true;
                });
            }

            return errorsOccurred ?
                new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, "Errors occurred while configuration updating") :
                new(SB_STATUS.SB_OK, true, true);
        }

        public SB_RESULT<bool> Remove() => RemoveBox(Name);

        public SB_RESULT<bool> LoadDefaults()
        {
            if (!Exists) return new SB_RESULT<bool>(SB_STATUS.SB_BOX_LOAD_DEFAULTS_BOX_NOT_EXISTS, false, false, "Can't load defaults because box is not exists. Use Create() instead");

            var boxDefaults = Config.GetBoxDefaults();
            var deleteConfig = Config.Edit(Config.Operations.Overwrite, Name, Config.Keys.Asterisk, "");
            if (boxDefaults.Result is false || boxDefaults.Data is null) return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while getting default settings or value is null");
            if (deleteConfig.Result is false) return new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, $"Error occurred while deleting old {Name} configuration");
            var errorsOccurred = false;

            foreach (var keyValuePair in boxDefaults.Data)
            {
                keyValuePair.Value.ForEach(value =>
                {
                    var editResult = Config.Edit(Config.Operations.Append, Name, keyValuePair.Key, value);
                    if (editResult.Result is false) errorsOccurred = true;
                });
            }

            return errorsOccurred ?
                new(SB_STATUS.SB_BOX_LOAD_DEFAULTS_ERROR, false, false, "Errors occurred while configuration updating") :
                new(SB_STATUS.SB_OK, true, true);
        }
    }

    /// <summary>
    /// Class with native sandboxie functions from <see cref="SbieDll"/>.
    /// </summary>
    public abstract class Api
    {
        private static readonly IntPtr SbieDllHandle = LoadLibrary(SbieDll.Path);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint ucchMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        private static T GetDelegateForFunctionPointer<T>(string functionName)
        {
            IntPtr dllFunctionPointer = GetProcAddress(SbieDllHandle, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(dllFunctionPointer);
        }

        public delegate IntPtr PSbieDllStartSbieSvc([MarshalAs(UnmanagedType.Bool)] bool retry);
        public static readonly PSbieDllStartSbieSvc StartSbieSvc = GetDelegateForFunctionPointer<PSbieDllStartSbieSvc>("SbieDll_StartSbieSvc");

        public delegate long PSbieApiEnumBoxes(long index, [MarshalAs(UnmanagedType.LPWStr)] string box_name);
        public static readonly PSbieApiEnumBoxes EnumBoxes = GetDelegateForFunctionPointer<PSbieApiEnumBoxes>("SbieApi_EnumBoxes");

        public delegate long PSbieApiQueryBoxPath(
            [MarshalAs(UnmanagedType.LPWStr)] string box_name,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder file_path,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder key_path,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ipc_path,
            ref uint file_path_len,
            ref uint key_path_len,
            ref uint ipc_path_len);
        public static readonly PSbieApiQueryBoxPath QueryBoxPath = GetDelegateForFunctionPointer<PSbieApiQueryBoxPath>("SbieApi_QueryBoxPath");

        public delegate long PSbieApiQueryProcessPath(
            IntPtr process_id,
            [MarshalAs(UnmanagedType.LPWStr)] string file_path,
            [MarshalAs(UnmanagedType.LPWStr)] string key_path,
            [MarshalAs(UnmanagedType.LPWStr)] string ipc_path,
            ref uint file_path_len,
            ref uint key_path_len,
            ref uint ipc_path_len);
        public static readonly PSbieApiQueryProcessPath QueryProcessPath = GetDelegateForFunctionPointer<PSbieApiQueryProcessPath>("SbieApi_QueryProcessPath");

        public delegate long PSbieApiEnumProcessEx(
            [MarshalAs(UnmanagedType.LPWStr)] string box_name,
            [MarshalAs(UnmanagedType.Bool)] bool all_sessions,
            uint which_session,
            [AllowNull][MarshalAs(UnmanagedType.LPArray)] uint[] boxed_pids,
            ref uint boxed_count);
        public static readonly PSbieApiEnumProcessEx EnumProcessEx = GetDelegateForFunctionPointer<PSbieApiEnumProcessEx>("SbieApi_EnumProcessEx");

        public delegate long PSbieApiQueryProcess(
            IntPtr process_id,
            [MarshalAs(UnmanagedType.LPWStr)] string box_name,
            [MarshalAs(UnmanagedType.LPWStr)] string image_name,
            [MarshalAs(UnmanagedType.LPWStr)] string sid_string,
            ref uint session_id);
        public static readonly PSbieApiQueryProcess QueryProcess = GetDelegateForFunctionPointer<PSbieApiQueryProcess>("SbieApi_QueryProcess");

        public delegate bool PSbieDllKillOne(IntPtr process_id);
        public static readonly PSbieDllKillOne KillOne = GetDelegateForFunctionPointer<PSbieDllKillOne>("SbieDll_KillOne");

        public delegate bool PSbieDllKillAll(uint session_id, [MarshalAs(UnmanagedType.LPWStr)] string box_name);
        public static readonly PSbieDllKillAll KillAll = GetDelegateForFunctionPointer<PSbieDllKillAll>("SbieDll_KillAll");

        public delegate long PSbieApiQueryConf(
            [MarshalAs(UnmanagedType.LPWStr)] string section_name,
            [MarshalAs(UnmanagedType.LPWStr)] string setting_name,
            uint setting_index,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value,
            uint value_len);
        public static readonly PSbieApiQueryConf QueryConf = GetDelegateForFunctionPointer<PSbieApiQueryConf>("SbieApi_QueryConf");

        public delegate long PSbieDllUpdateConf(
            char operation_code,
            [MarshalAs(UnmanagedType.LPWStr)] string password,
            [MarshalAs(UnmanagedType.LPWStr)] string section_name,
            [MarshalAs(UnmanagedType.LPWStr)] string setting_name,
            [MarshalAs(UnmanagedType.LPWStr)] string value);
        public static readonly PSbieDllUpdateConf UpdateConf = GetDelegateForFunctionPointer<PSbieDllUpdateConf>("SbieDll_UpdateConf");

        public delegate long PSbieApiReloadConf(uint session_id);
        public static readonly PSbieApiReloadConf ReloadConf = GetDelegateForFunctionPointer<PSbieApiReloadConf>("SbieApi_ReloadConf");

        public delegate IntPtr PSbieApiGetHomePath(
            [MarshalAs(UnmanagedType.LPWStr)] string NtPath,
            uint NtPathMaxLen,
            [MarshalAs(UnmanagedType.LPWStr)] string DosPath,
            uint DosPathMaxLen);
        public static readonly PSbieApiGetHomePath GetHomePath = GetDelegateForFunctionPointer<PSbieApiGetHomePath>("SbieApi_GetHomePath");

        public delegate bool PSbieDllRunSandboxed(
            [MarshalAs(UnmanagedType.LPWStr)] string box_name,
            [MarshalAs(UnmanagedType.LPWStr)] string cmd,
            [MarshalAs(UnmanagedType.LPWStr)] string dir,
            uint creation_flags,
            ref STARTUPINFO si,
            out PROCESS_INFORMATION pi);

        public static readonly PSbieDllRunSandboxed RunSandboxed = GetDelegateForFunctionPointer<PSbieDllRunSandboxed>("SbieDll_RunSandboxed");
    }
}