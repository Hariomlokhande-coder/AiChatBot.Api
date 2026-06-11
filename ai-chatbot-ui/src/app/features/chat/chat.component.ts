import { Component, OnInit, signal, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../core/services/chat.service';
import { ChatMessage } from '../../models/chat-message.model';
import { ChatRequest } from '../../models/chat-request.model';
import { SidebarComponent } from '../../shared/sidebar/sidebar.component';
import { MessageComponent } from '../../shared/message/message.component';
import { ChatInputComponent } from '../../shared/chat-input/chat-input.component';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, SidebarComponent, MessageComponent, ChatInputComponent],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  @ViewChild(SidebarComponent) sidebar?: SidebarComponent;

  readonly messages = signal<ChatMessage[]>([]);
  readonly message = signal('');
  readonly conversationId = signal('');
  readonly loading = signal(false);
  readonly sending = signal(false);
  readonly error = signal('');
  readonly typing = signal(false);
  readonly hasMore = signal(false);
  readonly loadingMore = signal(false);

  constructor(private readonly chatService: ChatService) {}

  ngOnInit(): void {
    // Automatically start a brand new conversation session every time the application opens
    this.startNewConversation();
  }

  ngAfterViewInit(): void {
    // Load sidebar conversations after view init
    setTimeout(() => this.sidebar?.load(), 0);
  }

  onSend(): void {
    // Deprecated: use onInputSend from ChatInputComponent
  }

  onInputSend(value: string): void {
    const trimmed = value.trim();
    if (!trimmed || this.sending()) return;

    if (!this.conversationId()) this.startNewConversation();

    const request: ChatRequest = {
      message: trimmed,
      conversationId: this.conversationId()
    };

    this.sending.set(true);
    this.error.set('');
    this.messages.update((list) => [...list, { role: 'user', text: trimmed }]);

    this.typing.set(true);
    this.chatService.sendMessage(request).subscribe({
      next: (response) => {
        if (response.success) {
          // simulate streaming: reveal assistant message progressively
          const full = response.message || '';
          let current = '';
          this.messages.update((list) => [...list, { role: 'ai', text: '' }]);

          const interval = setInterval(() => {
            const nextChar = full[current.length] ?? '';
            current += nextChar;
            // update last message
            this.messages.update((list) => {
              const copy = [...list];
              copy[copy.length - 1] = { role: 'ai', text: current };
              return copy;
            });

            if (current.length >= full.length) {
              clearInterval(interval);
              this.typing.set(false);
              this.scrollToBottom();
              // refresh sidebar to pick up new or updated conversation
              this.sidebar?.load();
            }
          }, 6);

          return;
        }

        this.error.set('The server rejected the message.');
        this.typing.set(false);
      },
      error: () => {
        this.error.set('Unable to send your message. Please try again.');
        this.typing.set(false);
        this.sending.set(false);
      },
      complete: () => this.sending.set(false)
    });
  }

  startNewConversation(): void {
    this.error.set('');
    const id = this.createConversationId();
    this.conversationId.set(id);
    this.saveConversationId(id);
    this.messages.set([]);
    this.message.set('');
    this.hasMore.set(false);
  }

  newChat(): void {
    this.startNewConversation();
    this.sidebar?.load();
  }

  onConversationSelected(id: string): void {
    this.conversationId.set(id);
    this.saveConversationId(id);
    this.fetchConversation();
  }

  private fetchConversation(): void {
    this.loading.set(true);
    this.error.set('');

    this.chatService.getConversationHistory(this.conversationId(), undefined, 10).subscribe({
      next: (res) => {
        this.messages.set(res.messages || []);
        this.hasMore.set(res.hasMore);
        this.scrollToBottom();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load conversation history.');
        this.loading.set(false);
      }
    });
  }

  loadMore(): void {
    if (this.loadingMore() || !this.hasMore()) return;

    this.loadingMore.set(true);
    const oldestMessage = this.messages()[0];
    const beforeTimestamp = oldestMessage?.timestamp;

    this.chatService.getConversationHistory(this.conversationId(), beforeTimestamp, 10).subscribe({
      next: (res) => {
        this.messages.update((list) => [...(res.messages || []), ...list]);
        this.hasMore.set(res.hasMore);
        this.preserveScrollAfterLoadMore();
        this.loadingMore.set(false);
      },
      error: () => {
        this.error.set('Unable to load older messages.');
        this.loadingMore.set(false);
      }
    });
  }

  private preserveScrollAfterLoadMore(): void {
    const chatElement = document.getElementById('chat-messages');
    if (!chatElement) return;

    const previousScrollHeight = chatElement.scrollHeight;
    setTimeout(() => {
      const currentScrollHeight = chatElement.scrollHeight;
      chatElement.scrollTop += (currentScrollHeight - previousScrollHeight);
    }, 0);
  }

  private createConversationId(): string {
    return typeof crypto !== 'undefined' && 'randomUUID' in crypto
      ? crypto.randomUUID()
      : `chat-${Date.now()}`;
  }

  private saveConversationId(conversationId: string): void {
    localStorage.setItem('chat.conversationId', conversationId);
  }

  private loadConversationId(): string {
    return localStorage.getItem('chat.conversationId') ?? '';
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const chatElement = document.getElementById('chat-messages');
      if (chatElement) {
        chatElement.scrollTop = chatElement.scrollHeight;
      }
    }, 0);
  }
}
