using OpenNGS.SDK.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class TestAuth : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return SignInWithOpenIdConnectAsync("oidc-eegames", "");
    }


    async Task SignInWithOpenIdConnectAsync(string idProviderName, string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(idProviderName, idToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

}
