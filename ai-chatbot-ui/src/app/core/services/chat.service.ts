import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChatMessage } from '../../models/chat-message.model';
import { ChatRequest } from '../../models/chat-request.model';
import { ChatResponse } from '../../models/chat-response.model';
import { Conversation } from '../../models/conversation.model';
import { environment } from '../../../environments/environment';
 
@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  sendMessage(request: ChatRequest): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.apiUrl}/send`, request);
  }

  getConversation(conversationId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/${conversationId}`);
  }

  getConversationHistory(
    conversationId: string,
    before?: string,
    pageSize: number = 10
  ): Observable<{ messages: ChatMessage[]; hasMore: boolean }> {
    let url = `${this.apiUrl}/${conversationId}/history?pageSize=${pageSize}`;
    if (before) {
      url += `&before=${encodeURIComponent(before)}`;
    }
    return this.http.get<{ messages: ChatMessage[]; hasMore: boolean }>(url);
  }

  getConversations(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(`${this.apiUrl}/conversations`);
  }

  deleteConversation(conversationId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${conversationId}`);
  }

  renameConversation(conversationId: string, title: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${conversationId}/rename`, { title });
  }
}
