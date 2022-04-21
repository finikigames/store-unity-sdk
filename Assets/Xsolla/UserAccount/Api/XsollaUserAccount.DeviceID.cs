using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_ADD_USERNAME_EMAIL =
			"https://login.xsolla.com/api/users/me/link_email_password?login_url={0}";

		private const string URL_GET_USERS_DEVICES =
			"https://login.xsolla.com/api/users/me/devices";

		private const string URL_DEVICES_LINKING =
			"https://login.xsolla.com/api/users/me/devices/{0}";

		/// <summary>
		/// Adds the username/email and password authentication to the existing user account. This call is used if the account is created via device ID or phone number.
		/// </summary>
		/// <remarks>Swagger method name:<c>Add username/email auth to account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-profile/add-username-email-auth-to-account/"/>.
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email.</param>
		/// <param name="promoEmailAgreement">Default: 1. User consent to receive the newsletter. Enum: 0 1</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void AddUsernameEmailAuthToAccount(string username, string password, string email, int? promoEmailAgreement = null, Action<bool> onSuccess = null, Action<Error> onError = null)
		{
			var requestBody = new AddUsernameAndEmailRequest(username, password, email, promoEmailAgreement);
			var loginUrl = RedirectUtils.GetRedirectUrl();
			var url = string.Format(URL_ADD_USERNAME_EMAIL, loginUrl);

			Action<AddUsernameAndEmailResponse> onComplete = response =>
			{
				onSuccess?.Invoke(response.email_confirmation_required);
			};

			WebRequestHelper.Instance.PostRequest<AddUsernameAndEmailResponse, AddUsernameAndEmailRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token.Instance),
				onComplete: onComplete,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => AddUsernameEmailAuthToAccount(username, password, email, promoEmailAgreement, onSuccess, onError)));
		}

		/// <summary>
		/// Gets a list of user�s devices.
		/// </summary>
		/// <remarks>Swagger method name:<c>Get user's devices</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/get-users-devices/"/>.
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserDevices(Action<List<UserDeviceInfo>> onSuccess = null, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest<List<UserDeviceInfo>>(SdkType.Login, URL_GET_USERS_DEVICES, WebRequestHeader.AuthHeader(Token.Instance),
				onComplete: onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetUserDevices(onSuccess, onError)));
		}

		/// <summary>
		/// Links the specified device to the user account. To enable authentication via device ID and linking, contact your Account Manager.
		/// </summary>
		/// <remarks>Swagger method name:<c>Link device to account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/link-device-to-account/"/>.
		/// <param name="deviceType">Type of the device.</param>
		/// <param name="device">Manufacturer and model name of the device.</param>
		/// <param name="deviceId">Device ID: For Android it is an ANDROID_ID constant. For iOS it is an identifierForVendor property.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void LinkDeviceToAccount(DeviceType deviceType, string device, string deviceId, Action onSuccess = null, Action<Error> onError = null)
		{
			var deviceTypeAsString = deviceType.ToString().ToLower();
			var requestBody = new LinkDeviceRequest(device, deviceId);
			var url = string.Format(URL_DEVICES_LINKING, deviceTypeAsString);

			WebRequestHelper.Instance.PostRequest<LinkDeviceRequest>(SdkType.Login, url, requestBody, WebRequestHeader.AuthHeader(Token.Instance),
				onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => LinkDeviceToAccount(deviceType, device, deviceId, onSuccess, onError)));
		}

		/// <summary>
		/// Unlinks the specified device from the user account. To enable authentication via device ID and unlinking, contact your Account Manager.
		/// </summary>
		/// <remarks>Swagger method name:<c>Unlink the device from account</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/devices/unlink-device-from-account/"/>.
		/// <param name="id">Device ID of the device you want to unlink. It is generated by the Xsolla Login server. It is not the same as the device_id parameter from the Auth via device ID (JWT and OAuth 2.0) call.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UnlinkDeviceFromAccount(int id, Action onSuccess = null, Action<Error> onError = null)
		{
			var url = string.Format(URL_DEVICES_LINKING, id);

			WebRequestHelper.Instance.DeleteRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(Token.Instance),
				onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UnlinkDeviceFromAccount(id, onSuccess, onError)));
		}
	}
}
