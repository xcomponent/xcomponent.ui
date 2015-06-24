using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FirstFloor.ModernUI.Presentation;
using XComponent.Common.UI.Properties;

namespace XComponent.Common.UI.DockingInteraction
{
    public static class CommonContextMenuItemFactory
    {
        public static IEnumerable<MenuItem> CreateStandardMenuItems(IInteractionParticipantWithContextMenu participant, Func<object, ParticipantIdentity> identityFromParameterAccessor, Func<object, IEnumerable<Dictionary<string, object>>> parameterAccessor)
        {
            IEnumerable<ParticipantIdentity> peers = participant.Mediator.GetParticipants(participant.ContextMenuPeerTypes).ToList();

            var standardMenuItems = new List<MenuItem>();

            if (participant.ContextMenuPeerTypes.Any())
            {
                MenuItem sendToMenuItem = GenerateSendToMenuItem(participant, peers, parameterAccessor, identityFromParameterAccessor);

                standardMenuItems.Add(sendToMenuItem);
            }

            if (participant.ContextMenuSlaveTypes.Any())
            {
                MenuItem enslaveMenuItem = GenerateEnslaveMenuItem(participant, peers, parameterAccessor, identityFromParameterAccessor);

                standardMenuItems.Add(enslaveMenuItem);
            }
            return standardMenuItems;
        }

        public static MenuItem GenerateEnslaveMenuItem(IInteractionParticipantWithContextMenu participant, IEnumerable<ParticipantIdentity> peers, Func<object, IEnumerable<Dictionary<string, object>>> parameterAccessor, Func<object, ParticipantIdentity> identityFromParameterAccessor)
        {
            var enslaveMenuItemChildren = new List<MenuItem>();
            var peersByViewType =
                peers.GroupBy(p => p.ParticipantType)
                    .Where(g => g.Key.Category != participant.Identity.ParticipantType.Category && participant.ContextMenuSlaveTypes.Contains(g.Key))
                    .SelectMany(g => g)
                    .GroupBy(identity => identity.ParticipantType.Category)
                    .ToList();

            foreach (IGrouping<string, ParticipantIdentity> groupOfPeers in peersByViewType)
            {
                if (!groupOfPeers.Any())
                {
                    continue;
                }

                MenuItem parent = new MenuItem();
                parent.Header = participant.Mediator.TranslateResource(groupOfPeers.Key);
                enslaveMenuItemChildren.Add(parent);

                foreach (ParticipantIdentity peer in groupOfPeers)
                {
                    var menuItem = CreateInteractionMenuItem(participant, peer, InteractionPattern.Enslave, parameterAccessor, identityFromParameterAccessor);

                    parent.Items.Add(menuItem);
                }
            }

            if (enslaveMenuItemChildren.Count == 0)
            {
                return null;
            }

            var enslaveMenuItem = new MenuItem();
            enslaveMenuItem.Header = Resources.Enslave;

            foreach (MenuItem enslaveMenuItemChild in enslaveMenuItemChildren)
            {
                enslaveMenuItem.Items.Add(enslaveMenuItemChild);
            }
            return enslaveMenuItem;

        }

        public static MenuItem GenerateSendToMenuItem(IInteractionParticipantWithContextMenu participant, IEnumerable<ParticipantIdentity> peers, Func<object, IEnumerable<Dictionary<string, object>>> parameterAccessor, Func<object, ParticipantIdentity> identityFromParameterAccessor)
        {
            var peersByCategory = peers.GroupBy(p => p.ParticipantType.Category).OrderBy(g => g.Key).ToList();

            var sendToMenuItemChildren = new List<MenuItem>();

            foreach (IGrouping<string, ParticipantIdentity> groupOfPeers in peersByCategory)
            {
                if (groupOfPeers.All(peer => ParticipantIdentity.UniqueNameParticipantTypeComparer.Equals(peer, participant.Identity)))
                {
                    continue;
                }

                var parent = new MenuItem();
                parent.Header = participant.Mediator.TranslateResource(groupOfPeers.Key);
                sendToMenuItemChildren.Add(parent);

                foreach (ParticipantIdentity peer in groupOfPeers)
                {
                    if (ParticipantIdentity.UniqueNameParticipantTypeComparer.Equals(peer, participant.Identity))
                    {
                        continue;
                    }

                    var menuItem = CreateInteractionMenuItem(participant, peer, InteractionPattern.PeerToPeer, parameterAccessor, identityFromParameterAccessor);
                    parent.Items.Add(menuItem);
                }
            }

            if (sendToMenuItemChildren.Count == 0)
            {
                return null;
            }

            var sendToMenuItem = new MenuItem();
            sendToMenuItem.Header = Resources.SendToMenuItemLabel;

            foreach (MenuItem sendToMenuItemChild in sendToMenuItemChildren)
            {
                sendToMenuItem.Items.Add(sendToMenuItemChild);
            }
            return sendToMenuItem;
        }

        private static MenuItem CreateInteractionMenuItem(IInteractionParticipantWithContextMenu participant, ParticipantIdentity peer, InteractionPattern interactionPattern, Func<object, IEnumerable<Dictionary<string, object>>> parameterAccessor, Func<object, ParticipantIdentity> identityFromParameterAccessor)
        {
            var menuItem = new MenuItem();

            menuItem.Header = peer.UniqueName;
            menuItem.Command = new RelayCommand(
                param =>
                {
                    ParticipantIdentity selectedPeer = identityFromParameterAccessor(param);
                    if (selectedPeer == null)
                    {
                        return;
                    }

                    IEnumerable<Dictionary<string, object>> parameters = parameterAccessor(selectedPeer);
                    var interaction = new Interaction(interactionPattern, participant.Identity, selectedPeer, parameters, CommonActions.UpdateDataSource);
                    participant.Mediator.RunInteraction(interaction);
                },
                param => true);

            menuItem.CommandParameter = peer;
            return menuItem;
        }
    }
}
