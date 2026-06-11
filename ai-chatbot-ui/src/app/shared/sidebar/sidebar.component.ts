import { Component, signal, EventEmitter, Output, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Conversation } from '../../models/conversation.model';
import { ChatService } from '../../core/services/chat.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  @Input() activeConversationId = '';
  @Output() conversationSelected = new EventEmitter<string>();
  @Output() createNew = new EventEmitter<void>();

  readonly conversations = signal<Conversation[]>([]);
  readonly loading = signal(false);
  readonly query = signal('');

  get filtered() {
    const q = this.query().toLowerCase().trim();
    if (!q) return this.conversations();
    return this.conversations().filter(c => c.title.toLowerCase().includes(q));
  }

  readonly editingId = signal('');
  readonly editingTitle = signal('');

  deleteConversation(id: string): void {
    if (!confirm('Delete this conversation? This cannot be undone.')) return;
    this.loading.set(true);
    this.chatService.deleteConversation(id).subscribe({
      next: () => this.load(),
      error: () => this.load()
    });
  }

  startEdit(c: Conversation): void {
    this.editingId.set(c.conversationId);
    this.editingTitle.set(c.title);
  }

  saveEdit(): void {
    const id = this.editingId();
    const title = this.editingTitle().trim();
    if (!id || !title) return;
    this.loading.set(true);
    this.chatService.renameConversation(id, title).subscribe({
      next: () => {
        this.editingId.set('');
        this.editingTitle.set('');
        this.load();
      },
      error: () => this.load()
    });
  }

  cancelEdit(): void {
    this.editingId.set('');
    this.editingTitle.set('');
  }

  constructor(private readonly chatService: ChatService) {
    // do not auto-load in constructor; caller should call load()
  }

  load(): void {
    this.loading.set(true);
    this.chatService.getConversations().subscribe({
      next: (list) => {
        this.conversations.set(list || []);
        this.loading.set(false);
      },
      error: () => {
        this.conversations.set([]);
        this.loading.set(false);
      }
    });
  }

  selectConversation(id: string): void {
    this.conversationSelected.emit(id);
  }

  newConversation(): void {
    this.createNew.emit();
  }
}
