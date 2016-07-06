﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    public class LobbyServerList : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;

        protected int currentPage = 0;
        protected int previousPage = 0;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        public void OnEnable()
        {
            currentPage = 0;
            previousPage = 0;

            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            noServerFound.SetActive(false);

            RequestPage(0);
        }

        public void OnGUIMatchList(ListMatchResponse response)
        {
            if (response.matches.Count == 0)
            {
                if (currentPage == 0)
                {
                    noServerFound.SetActive(true);
                }

                currentPage = previousPage;
               
                return;
            }

            noServerFound.SetActive(false);
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            for (int i = 0; i < response.matches.Count; ++i)
            {
                GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                o.GetComponent<LobbyServerEntry>().Populate(response.matches[i], lobbyManager, (i%2 == 0) ? OddServerColor : EvenServerColor);

                o.transform.SetParent(serverListRect, false);
            }
        }

        public void ChangePage(int dir)
        {
            int newPage = Mathf.Max(0, currentPage + dir);
            if (noServerFound.activeSelf)
                newPage = 0;

            RequestPage(newPage);
        }

        public void RequestPage(int page)
        {
            previousPage = currentPage;
            currentPage = page;
            lobbyManager.matchMaker.ListMatches(page, 6, "", OnGUIMatchList);
        }
    }
}