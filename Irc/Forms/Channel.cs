﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irc.Irc;

namespace Irc.Forms
{
    public partial class Channel : UserControl
    {
        public delegate void OnSendMessage(string identify, string channel, string message);
        private OnSendMessage OSM;
        private Dictionary<string, Dictionary<string, string>> topics = new Dictionary<string, Dictionary<string, string>>();

        public Channel(Form1 main)
        {
            InitializeComponent(main);
            this.channelButton1.AddEvent(OnSend);
        }

        public void SetTopic(string identify, string channel, string topic)
        {
            this.ShowLine(identify, channel, null, "Topic: " + topic);
            if (!this.topics.ContainsKey(identify))
                this.topics.Add(identify, new Dictionary<string, string>());
            if (!this.topics[identify].ContainsKey(channel))
                this.topics[identify].Add(channel, topic);
            else
                this.topics[identify][channel] = topic;
        }

        public string GetCurrentTopic()
        {
            if(this.topics.ContainsKey(this.message1.GetSelectedIdentify()) && this.topics[this.message1.GetSelectedIdentify()].ContainsKey(this.message1.GetSelectedChannel()))
            {
                return this.topics[this.message1.GetSelectedIdentify()][this.message1.GetSelectedChannel()];
            }
            return null;
        }

        public void Select(Form1 form, string identify, string channel)
        {
            this.message1.Select(identify, channel);
            this.UserList.Select(identify, channel);
            form.SetTitle();
        }

        public bool ChannelExists(string identify, string channel)
        {
            return this.message1.ChannelExists(identify, channel);
        }

        public void ShowLine(string identify, string channel, string from, string message)
        {
            this.message1.ShowLine(identify, channel, from, message);
        }

        public void AppendUser(string indentify, string channel, UserInfo info)
        {
            this.UserList.AppendUser(indentify, channel, info);
        }

        public bool RemoveUser(string identify, string channel, string nick)
        {
            return this.UserList.RemoveUser(identify, channel, nick);
        }

        public void SetSendMessageEvent(OnSendMessage osm)
        {
            this.OSM = osm;
        }

        public void CloseServer(string identify)
        {
            this.UserList.RemoveServer(identify);
            this.message1.RemoveServer(identify);
        }

        public bool CloseChannel(string identify, string channel)
        {
            this.UserList.RemoveChannel(identify, channel);
            return this.message1.RemoveChannel(identify, channel);
        }

        public string SelectedIdentify()
        {
            return this.message1.GetSelectedIdentify();
        }

        public string SelectedChannel()
        {
            return this.message1.GetSelectedChannel();
        }

        public string GetTopServer()
        {
            return this.message1.GetTopServer();
        }

        public string GetTopChannel(string identify)
        {
            return this.message1.GetTopChannel(identify);
        }

        private void OnSend(string message)
        {
            if(this.OSM != null)
            {
                this.OSM(this.message1.GetSelectedIdentify(), this.message1.GetSelectedChannel(), message);
            }
        }
    }
}