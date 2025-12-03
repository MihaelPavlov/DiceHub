import { Component, EventEmitter, Input, input, Output } from '@angular/core';
import { ChallengeType } from '../challenge-type.enum';

@Component({
    selector: 'app-challenge-type-toggle',
    templateUrl: './challenge-type-toggle.component.html',
    styleUrls: ['./challenge-type-toggle.component.scss'],
    standalone: false
})
export class ChallengeTypeToggleComponent {
  public activeTab: ChallengeType = ChallengeType.Game;
  public readonly ChallengeType = ChallengeType;
  @Input() initialTab: ChallengeType = ChallengeType.Game;
  @Output() tabChange = new EventEmitter<ChallengeType>();
  constructor() {
    this.activeTab = this.initialTab;
  }
  public switchTab(tab: ChallengeType): void {
    this.activeTab = tab;
    this.tabChange.emit(this.activeTab);
  }
}
