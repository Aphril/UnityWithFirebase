using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using Firebase.Auth;
using Firebase.Extensions;

public class AuthWithGoogle : MonoBehaviour
{
	public FirebaseAuth firebaseAuth;
	public FirebaseUser firebaseUser;
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
	private GoogleSignInConfiguration configuration;
	[SerializeField] string WebID = "<WebId>";
	int registeredUser;

	void Awake()
	{
		configuration = new GoogleSignInConfiguration
		{
			WebClientId = WebID,
			RequestEmail = true,
			RequestIdToken = true
		};

	}

	void Start()
	{
    registeredUser = PlayerPrefs.GetInt("itIsRegistered", 0);
    
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
      dependencyStatus = task.Result;
      if (dependencyStatus == DependencyStatus.Available)
      {
        firebaseAuth = DefaultInstance;
        
        if (registeredUser == 1)
		    {
          firebaseUser = firebaseAuth.CurrentUser;
          Debug.Log("The user is already registered.");
          Debug.Log("The user is: " + firebaseUser.Email);
		    }
      }
    }	
	}

  // Call this method to register the user.
	public void SignIn()
	{
    #if UNITY_ANDROID
		  SignInWithGoogle();
    #endif
	}

	void SignInWithGoogle()
	{
		GoogleSignIn.Configuration = configuration;
		GoogleSignIn.Configuration.RequestEmail = true;
		GoogleSignIn.Configuration.UseGameSignIn = false;
		GoogleSignIn.Configuration.RequestIdToken = true;
		GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
		  OnGoogleAuthenticationFinished);
	}

	void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> task)
	{
		if (task.IsFaulted)
		{
			using (IEnumerator<System.Exception> enumerator =
					task.Exception.InnerExceptions.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
				}
			}
		}
		else if (task.IsCanceled)
		{
		}
		else
		{
			//Collect the user's credentials and register them with the firebase server.
      
			Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
			mAuth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
			{
				FirebaseUser = firebaseAuth.CurrentUser;
        Debug.Log(firebaseUser.Email);
				PlayerPrefs.SetInt("itIsRegistered", 1);
			});
		}		
	}
}
