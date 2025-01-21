using AppleAuth;
using AppleAuth.Native;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AppleAuth.Interfaces;
using AppleAuth.Enums;
using Firebase.Auth;

public static class AppleManager
{
    private static IAppleAuthManager appleAuthManager;
    public static bool IsInit;

    public static void Init(Action _callBack)
    {
        if (IsInit)
        {
            _callBack?.Invoke();
            return;
        }
        
        var _deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(_deserializer);
        IsInit = true;
        _callBack?.Invoke();
    }

    public static void Login(Action<Credential> _loginSuccess, Action<string> _loginFailed)
    {
        string _rawNonce = GenerateRandomString(32);
        var _nonce = GenerateSHA256NonceFromRawNonce(_rawNonce);

        var _loginArgs = new AppleAuthLoginArgs(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            _nonce);

        appleAuthManager.LoginWithAppleId(
            _loginArgs,
            _credential =>
            {
                var _appleIdCredential = _credential as IAppleIDCredential;
                if (_appleIdCredential != null)
                {
                    var _identityToken = Encoding.UTF8.GetString(
                        _appleIdCredential.IdentityToken,
                        0,
                        _appleIdCredential.IdentityToken.Length);

                    var _authorizationCode = Encoding.UTF8.GetString(
                        _appleIdCredential.AuthorizationCode,
                        0,
                        _appleIdCredential.AuthorizationCode.Length);

                    var _firebaseCredential = OAuthProvider.GetCredential(
                    "apple.com",
                    _identityToken,
                    _rawNonce,
                    _authorizationCode);

                    _loginSuccess?.Invoke(_firebaseCredential);
                }
            },
            _error =>
            {
                // Something went wrong
                _loginFailed?.Invoke("Something went wrong:\n"+_error);
            });
    }

    static string GenerateRandomString(int _length)
    {
        if (_length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        string _charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var _cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var _result = string.Empty;
        var _remainingLength = _length;

        var _randomNumberHolder = new byte[1];
        while (_remainingLength > 0)
        {
            var _randomNumbers = new List<int>(16);
            for (var _randomNumberCount = 0; _randomNumberCount < 16; _randomNumberCount++)
            {
                _cryptographicallySecureRandomNumberGenerator.GetBytes(_randomNumberHolder);
                _randomNumbers.Add(_randomNumberHolder[0]);
            }

            for (var _randomNumberIndex = 0; _randomNumberIndex < _randomNumbers.Count; _randomNumberIndex++)
            {
                if (_remainingLength == 0)
                {
                    break;
                }

                var _randomNumber = _randomNumbers[_randomNumberIndex];
                if (_randomNumber < _charset.Length)
                {
                    _result += _charset[_randomNumber];
                    _remainingLength--;
                }
            }
        }

        return _result;
    }

    static string GenerateSHA256NonceFromRawNonce(string _rawNonce)
    {
        var _sha = new SHA256Managed();
        var _utf8RawNonce = Encoding.UTF8.GetBytes(_rawNonce);
        var _hash = _sha.ComputeHash(_utf8RawNonce);

        var _result = string.Empty;
        for (var _i = 0; _i < _hash.Length; _i++)
        {
            _result += _hash[_i].ToString("x2");
        }

        return _result;
    }

    public static void Update()
    {
        appleAuthManager.Update();
    }
}
