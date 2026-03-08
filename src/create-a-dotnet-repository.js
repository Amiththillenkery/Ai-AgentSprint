import { Request, Response, NextFunction } from 'express';
import { Repository } from '@/models/repository';
import { AppSettings } from '@/config/appsettings';
import { GenericExceptionMiddleware } from '@/middleware/genericExceptionMiddleware';

export class DotnetRepositoryService {
  private repositories: Repository[] = [];
  private appSettings: AppSettings;

  constructor(appSettings: AppSettings) {
    this.appSettings = appSettings;
  }

  public async createRepository(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { name, description } = req.body;
      const newRepo: Repository = { id: this.generateId(), name, description };
      this.repositories.push(newRepo);
      res.status(201).json(newRepo);
    } catch (error) {
      next(new GenericExceptionMiddleware('Error creating repository', error));
    }
  }

  public async getRepository(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const repo = this.repositories.find(r => r.id === id);
      if (!repo) {
        res.status(404).json({ message: 'Repository not found' });
        return;
      }
      res.status(200).json(repo);
    } catch (error) {
      next(new GenericExceptionMiddleware('Error retrieving repository', error));
    }
  }

  public async updateRepository(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const { name, description } = req.body;
      const repoIndex = this.repositories.findIndex(r => r.id === id);
      if (repoIndex === -1) {
        res.status(404).json({ message: 'Repository not found' });
        return;
      }
      this.repositories[repoIndex] = { ...this.repositories[repoIndex], name, description };
      res.status(200).json(this.repositories[repoIndex]);
    } catch (error) {
      next(new GenericExceptionMiddleware('Error updating repository', error));
    }
  }

  public async deleteRepository(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const repoIndex = this.repositories.findIndex(r => r.id === id);
      if (repoIndex === -1) {
        res.status(404).json({ message: 'Repository not found' });
        return;
      }
      this.repositories.splice(repoIndex, 1);
      res.status(204).send();
    } catch (error) {
      next(new GenericExceptionMiddleware('Error deleting repository', error));
    }
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }
}