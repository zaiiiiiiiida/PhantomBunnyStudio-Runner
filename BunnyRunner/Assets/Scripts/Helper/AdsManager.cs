using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

public class AdsManager : MonoBehaviour
{
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";

    private BannerView _bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd _rewardedAdDouble;

    PlayerManager playerManager;
    PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found!");
        }

        MobileAds.Initialize(initStatus =>
        {
            CreateBannerView();
            LoadAd();
            LoadRewardedAd();
        });
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
    }

    public void LoadAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };

        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };

        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };

        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };

        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };

        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };

        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAdDouble != null)
        {
            _rewardedAdDouble.Destroy();
            _rewardedAdDouble = null;
        }

        Debug.Log("Loading the rewarded ad.");

        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAdDouble = ad;
                RegisterEventHandlers(_rewardedAdDouble);
            });
    }

    public void ShowRewardedAdDoubleScore()
    {
        if (_rewardedAdDouble != null && _rewardedAdDouble.CanShowAd())
        {
            _rewardedAdDouble.Show((Reward reward) =>
            {
                Debug.Log("Rewarded ad shown, rewarding the player.");
                if (playerManager != null)
                {
                    playerManager.DoubleEarnedScore();
                }
                else
                {
                    Debug.LogWarning("PlayerManager not found!");
                }
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready or cannot show ad.");
        }
    }

    public void ShowRewardedAdRevive()
    {
        if (_rewardedAdDouble != null && _rewardedAdDouble.CanShowAd())
        {
            _rewardedAdDouble.Show((Reward reward) =>
            {
                Debug.Log("Rewarded ad shown, rewarding the player.");
                if (playerHealth != null)
                {
                    playerHealth.Revive();
                }
                else
                {
                    Debug.LogWarning("PlayerHealth not found!");
                }
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready or cannot show ad.");
        }
    }

    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            LoadRewardedAd();
        };
    }
}
