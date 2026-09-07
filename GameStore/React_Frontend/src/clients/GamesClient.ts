import { CommandResult } from '../models/CommandResult';
import { GameDetails } from '../models/GameDetails';
import { GamesPage } from '../models/GamesPage';

class GamesClient {
  private baseUrl = '';

  async getGamesAsync(pageNumber: number, pageSize: number, nameSearch?: string): Promise<GamesPage> {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    if (nameSearch) {
      params.set('name', nameSearch);
    }

    const response = await this.fetchWithHandling(`${this.baseUrl}/games?${params.toString()}`);
    if (!response.ok) {
      const errorMessages = await this.handleFetchError(response);
      throw new Error(errorMessages.join('\n'));
    }

    const page: GamesPage = await response.json();

    page.data = page.data.map((game) => {
      const date = new Date(game.releaseDate);
      const formattedDate = `${String(date.getUTCMonth() + 1).padStart(2, '0')}/${String(date.getUTCDate()).padStart(2, '0')}/${date.getUTCFullYear()}`;
      return { ...game, releaseDate: formattedDate };
    });

    return page;
  }

  async addGameAsync(game: GameDetails, imageFile?: File): Promise<CommandResult> {
    const response = await this.fetchWithHandling(`${this.baseUrl}/games`, {
      method: 'POST',
      body: this.toFormData(game, imageFile),
    });

    if (!response.ok) {
      const errorMessages = await this.handleFetchError(response);
      return { succeeded: false, errors: errorMessages };
    }

    return { succeeded: true, errors: [] };
  }

  async getGameAsync(id: string): Promise<GameDetails> {
    const response = await this.fetchWithHandling(`${this.baseUrl}/games/${id}`);

    if (!response.ok) {
      const errorMessages = await this.handleFetchError(response);
      throw new Error(errorMessages.join('\n'));
    }

    return await response.json();
  }

  async updateGameAsync(updatedGame: GameDetails, imageFile?: File): Promise<CommandResult> {
    const response = await this.fetchWithHandling(`${this.baseUrl}/games/${updatedGame.id}`, {
      method: 'PUT',
      body: this.toFormData(updatedGame, imageFile),
    });

    if (!response.ok) {
      const errorMessages = await this.handleFetchError(response);
      return { succeeded: false, errors: errorMessages };
    }

    return { succeeded: true, errors: [] };
  }

  async deleteGameAsync(id: string): Promise<CommandResult> {
    const response = await this.fetchWithHandling(`${this.baseUrl}/games/${id}`, {
      method: 'DELETE',
    });

    if (!response.ok) {
      const errorMessages = await this.handleFetchError(response);
      return { succeeded: false, errors: errorMessages };
    }

    return { succeeded: true, errors: [] };
  }

  private toFormData(game: GameDetails, imageFile?: File): FormData {
    const formData = new FormData();
    formData.append('Name', game.name);
    formData.append('GenreId', game.genreId ?? '');
    formData.append('Price', game.price.toString());
    formData.append('ReleaseDate', game.releaseDate);
    formData.append('Description', game.description);
    if (imageFile) {
      formData.append('ImageFile', imageFile, imageFile.name);
    }
    return formData;
  }

  private async fetchWithHandling(url: string, options?: RequestInit): Promise<Response> {
    try {
      const response = await fetch(url, options);
      return response;
    } catch (error) {
      if (error instanceof TypeError) {
        throw new Error('We are currently experiencing issues loading the data. Please try again later.');
      }
      throw error;
    }
  }

  private async handleFetchError(response: Response): Promise<string[]> {
    let errorMessages: string[] = ['Unknown error'];
    try {
      const errorData = await response.json();
      if (errorData.title) {
        errorMessages = [errorData.title];
        if (errorData.errors && Array.isArray(errorData.errors)) {
          errorMessages = errorMessages.concat(errorData.errors);
        }
      } else if (errorData.errors && Array.isArray(errorData.errors)) {
        errorMessages = errorData.errors;
      } else if (errorData.detail) {
        errorMessages = [errorData.detail];
      }
    } catch (e) {
      console.error('Error parsing error response:', e);
    }
    return errorMessages;
  }
}

export default GamesClient;
