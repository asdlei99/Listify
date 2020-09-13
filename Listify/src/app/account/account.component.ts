import { ConfirmationmodalService } from './../services/confirmationmodal.service';
import { Router } from '@angular/router';
import { IApplicationUserRequest, IPurchasableItem, IPurchasableLineItem } from './../interfaces';
import { HubService } from './../services/hub.service';
import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CartService } from '../services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationmodalComponent } from '../shared/confirmationmodal/confirmationmodal.component';
import { GlobalsService } from '../services/globals.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit, OnDestroy {

  id: string;
  username: string;
  roomCode: string;
  roomTitle: string;
  roomKey: string;
  playlistSongCount: number;
  playlistCount: number;
  allowRequests: boolean;
  isLocked: boolean;
  isPublic: boolean;

  disableAttr = false;
  private _nextRequestType: NextRequestType;

  $userInformationSubscription: Subscription;
  $purchasableItemsSubscription: Subscription;
  $purchasableItemSubscription: Subscription;

  constructor(
    private router: Router,
    private hubService: HubService,
    private cartService: CartService,
    private globalsService: GlobalsService,
    private toastrService: ToastrService,
    private confirmationModal: MatDialog,
    private confirmationModalService: ConfirmationmodalService) {
      this.$userInformationSubscription = this.hubService.getApplicationUser().subscribe(user => {
        this.id = user.id;
        this.username = user.username;
        this.playlistSongCount = user.playlistSongCount;
        this.playlistCount = user.playlistCountMax;
        this.roomCode = user.room.roomCode;
        this.isLocked = user.room.isRoomLocked;
        this.isPublic = user.room.isRoomPublic;
        this.roomKey = user.room.roomKey;
        this.roomTitle = user.room.roomTitle;
        this.allowRequests = user.room.allowRequests;
      });

      this.$purchasableItemsSubscription = this.hubService.getPurchasableItems().subscribe(purchasableItems => {
        let selectedItem: IPurchasableItem;

        switch (this._nextRequestType) {
          case NextRequestType.Playlist:
          // tslint:disable-next-line:max-line-length
          selectedItem = purchasableItems.filter(x => x.purchasableItemType === this.globalsService.getPurchasableItemType('Playlist') && x.quantity === 1)[0];
          break;

          case NextRequestType.PlaylistSongs:
          // tslint:disable-next-line:max-line-length
          selectedItem = purchasableItems.filter(x => x.purchasableItemType === this.globalsService.getPurchasableItemType('PlaylistSongs') && x.quantity === 15)[0];
          break;
        }

        if (selectedItem) {
          this.hubService.requestPurchasableItem(selectedItem.id);
        }
      });

      this.$purchasableItemSubscription = this.hubService.getPurchasableItem().subscribe(purchasableItem => {
        const purchasableLineItem: IPurchasableLineItem = {
          purchasableItem: purchasableItem,
          orderQuantity: 1
        };
        this.addPurchasableItemToCard(purchasableLineItem);
      });
    }

  ngOnInit(): void {
    this.hubService.requestApplicationUserInformation();
  }

  ngOnDestroy(): void {
    this.$userInformationSubscription.unsubscribe();
    this.$purchasableItemsSubscription.unsubscribe();
    this.$purchasableItemSubscription.unsubscribe();
  }

  changeUsername(): void {
    // just make the input field editable and save the request
    this.disableAttr = true;
  }

  saveApplicationUserInfo(): void {

    if (this.roomCode === undefined || this.roomCode === null || this.roomCode === '') {
      this.toastrService.error('You have selected and invalid room code, please change the room code and try to save again', 'Invalid Room Code');

    }else if (this.roomTitle === undefined || this.roomTitle === null || this.roomTitle === '') {
      this.toastrService.error('You have entered and invalid room Title, please change the room title and try to save again', 'Invalid Room Title');

    }else if (this.isLocked && (this.roomKey === undefined || this.roomKey === null || this.roomKey === '')) {
      this.toastrService.error('You have selected to lock the room but have not provided a room Key, please enter room key', 'Invalid Room Key');

    }else {

      this.confirmationModalService.setConfirmationModalMessage('update your account information');

      const confirmationModal = this.confirmationModal.open(ConfirmationmodalComponent, {
        width: '250px',
        data: {isConfirmed: false}
      });

      confirmationModal.afterClosed().subscribe(result => {
        if (result !== undefined) {
          const request: IApplicationUserRequest = {
            id: this.id,
            username: this.username,
            roomCode: this.roomCode,
            roomTitle: this.roomTitle,
            roomKey: this.roomKey,
            allowRequests: this.allowRequests,
            isRoomLocked: this.isLocked,
            isRoomPublic: this.isPublic,
            chatColor: ''
          };

          this.hubService.updateApplicationUserInformation(request);

          this.router.navigate(['/', 'account']);

          this.toastrService.success('You have updated your account information successfully', 'Updated Successfully');
        }
      });
    }
  }

  addPlaylistSongCount(): void {
    this._nextRequestType = NextRequestType.PlaylistSongs;
    this.hubService.requestPurchasableItems();
  }

  addPlaylistCount(): void {
    this._nextRequestType = NextRequestType.Playlist;
    this.hubService.requestPurchasableItems();
  }

  addPurchasableItemToCard(purchasableLineItem: IPurchasableLineItem): void {
    this.cartService.addPurchasableItemToCart(purchasableLineItem);

    this.router.navigate(['/', 'cart']);

    // tslint:disable-next-line:max-line-length
    this.toastrService.success('You have added a ' + purchasableLineItem.purchasableItem.purchasableItemName + ' to your cat', 'Add Success');
  }
}

enum NextRequestType {
  Playlist,
  PlaylistSongs
}
