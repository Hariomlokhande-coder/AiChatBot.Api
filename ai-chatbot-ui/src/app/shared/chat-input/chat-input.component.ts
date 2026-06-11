import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat-input.component.html',
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent {
  @Input() disabled = false;
  @Output() send = new EventEmitter<string>();

  text = signal('');

  onInput(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
    this.text.set(target.value);
    this.adjustHeight(target);
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.trySend();
    }
  }

  trySend(): void {
    const value = this.text().trim();
    if (!value || this.disabled) return;
    this.send.emit(value);
    this.text.set('');
    
    // Reset heights
    const el = document.querySelector('.composer-textarea') as HTMLTextAreaElement;
    if (el) {
      el.style.height = 'auto';
    }
  }

  private adjustHeight(el: HTMLTextAreaElement): void {
    el.style.height = 'auto';
    el.style.height = `${el.scrollHeight}px`;
  }
}
