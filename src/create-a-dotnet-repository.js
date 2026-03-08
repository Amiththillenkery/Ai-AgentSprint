import { Request, Response, NextFunction } from 'express';
import { Repository } from '@/models/repository';
import { AppSettings } from '@/config/appsettings';
import { GenericExceptionMiddleware } from '@/middleware/genericExceptionMiddleware';

export class DotnetRepositoryService {
  private repositories: Repository[] = [];

  constructor(private appSettings: AppSettings) {}

  public getAllRepositories(req: Request, res: Response, next: NextFunction): void {
    try {
      res.status(200).json(this.repositories);
    } catch (error) {
      next(new GenericExceptionMiddleware(error));
    }
  }

  public getRepositoryById(req: Request, res: Response, next: NextFunction): void {
    try {
      const id = req.params.id;
      const repository = this.repositories.find(repo => repo.id === id);
      if (!repository) {
        res.status(404).send('Repository not found');
      } else {
        res.status(200).json(repository);
      }
    } catch (error) {
      next(new GenericExceptionMiddleware(error));
    }
  }

  public createRepository(req: Request, res: Response, next: NextFunction): void {
    try {
      const newRepository: Repository = req.body;
      this.repositories.push(newRepository);
      res.status(201).json(newRepository);
    } catch (error) {
      next(new GenericExceptionMiddleware(error));
    }
  }

  public updateRepository(req: Request, res: Response, next: NextFunction): void {
    try {
      const id = req.params.id;
      const repositoryIndex = this.repositories.findIndex(repo => repo.id === id);
      if (repositoryIndex === -1) {
        res.status(404).send('Repository not found');
      } else {
        this.repositories[repositoryIndex] = req.body;
        res.status(200).json(this.repositories[repositoryIndex]);
      }
    } catch (error) {
      next(new GenericExceptionMiddleware(error));
    }
  }

  public deleteRepository(req: Request, res: Response, next: NextFunction): void {
    try {
      const id = req.params.id;
      const repositoryIndex = this.repositories.findIndex(repo => repo.id === id);
      if (repositoryIndex === -1) {
        res.status(404).send('Repository not found');
      } else {
        this.repositories.splice(repositoryIndex, 1);
        res.status(204).send();
      }
    } catch (error) {
      next(new GenericExceptionMiddleware(error));
    }
  }
}