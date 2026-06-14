import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  const mockMovies = [
    { id: 1, title: 'Inception', rating: 9 },
    { id: 2, title: 'Avatar', rating: 7 },
    { id: 3, title: 'Gladiator', rating: 4 }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, AppComponent],
      providers: [ChangeDetectorRef]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    vi.restoreAllMocks();
  });

  it('should initialize and load movies/average on ngOnInit', () => {
    component.ngOnInit();

    const reqMovies = httpMock.expectOne(component.apiUrl);
    expect(reqMovies.request.method).toBe('GET');
    reqMovies.flush(mockMovies);

    const reqAvg = httpMock.expectOne(`${component.apiUrl}/average`);
    expect(reqAvg.request.method).toBe('GET');
    reqAvg.flush(6.67);

    expect(component.movies).toEqual(mockMovies);
    expect(component.average).toBe(6.67);
  });

  it('should add a movie and refresh data correctly', () => {
    component.title = 'Interstellar';
    component.rating = 10;

    component.addMovie();

    const reqPost = httpMock.expectOne(component.apiUrl);
    expect(reqPost.request.method).toBe('POST');
    expect(reqPost.request.body).toEqual({ title: 'Interstellar', rating: 10 });
    reqPost.flush({});

    const reqMovies = httpMock.expectOne(component.apiUrl);
    reqMovies.flush(mockMovies);

    const reqAvg = httpMock.expectOne(`${component.apiUrl}/average`);
    reqAvg.flush(7);

    expect(component.title).toBe('');
    expect(component.rating).toBe(1);
  });

  it('should cancel deleteMovie if user declines confirm dialog', () => {
    const confirmSpy = vi.spyOn(window, 'confirm').mockReturnValue(false);

    component.deleteMovie(1);

    httpMock.expectNone(`${component.apiUrl}/1`);
    expect(confirmSpy).toHaveBeenCalled();
  });

  it('should process deleteMovie correctly if user accepts confirm dialog', () => {
    const confirmSpy = vi.spyOn(window, 'confirm').mockReturnValue(true);
    component.movies = [...mockMovies];

    component.deleteMovie(1);

    const reqDelete = httpMock.expectOne(`${component.apiUrl}/1`);
    expect(reqDelete.request.method).toBe('DELETE');
    reqDelete.flush({});

    const reqAvg = httpMock.expectOne(`${component.apiUrl}/average`);
    reqAvg.flush(5.5);

    expect(component.movies.length).toBe(2);
    expect(component.movies.find(movie => movie.id === 1)).toBeUndefined();
    expect(confirmSpy).toHaveBeenCalled();
  });

  describe('Filtering and Sorting Matrix (filteredMovies)', () => {
    beforeEach(() => {
      component.movies = [
        { id: 1, title: 'B Movie', rating: 5 },
        { id: 2, title: 'C Movie', rating: 9 },
        { id: 3, title: 'A Movie', rating: 2 }
      ];
    });

    it('should filter movies based on searchText (case-insensitive)', () => {
      component.searchText = 'b mo';
      const results = component.filteredMovies();
      expect(results.length).toBe(1);
      expect(results[0].title).toBe('B Movie');
    });

    it('should sort by ratingHigh correctly', () => {
      component.sortOption = 'ratingHigh';
      const results = component.filteredMovies();
      expect(results[0].rating).toBe(9);
      expect(results[2].rating).toBe(2);
    });

    it('should sort by ratingLow correctly', () => {
      component.sortOption = 'ratingLow';
      const results = component.filteredMovies();
      expect(results[0].rating).toBe(2);
      expect(results[2].rating).toBe(9);
    });

    it('should sort alphabetically A-Z via az', () => {
      component.sortOption = 'az';
      const results = component.filteredMovies();
      expect(results[0].title).toBe('A Movie');
      expect(results[2].title).toBe('C Movie');
    });

    it('should sort alphabetically Z-A via za', () => {
      component.sortOption = 'za';
      const results = component.filteredMovies();
      expect(results[0].title).toBe('C Movie');
      expect(results[2].title).toBe('A Movie');
    });

    it('should sort by id ascending via oldest', () => {
      component.sortOption = 'oldest';
      const results = component.filteredMovies();
      expect(results[0].id).toBe(1);
      expect(results[2].id).toBe(3);
    });

    it('should fall back to newest (id descending) by default', () => {
      component.sortOption = 'newest';
      const results = component.filteredMovies();
      expect(results[0].id).toBe(3);
      expect(results[2].id).toBe(1);
    });
  });

  describe('ratingClass styling branches', () => {
    it('should return green when rating >= 8', () => {
      expect(component.ratingClass(8)).toBe('green');
      expect(component.ratingClass(10)).toBe('green');
    });

    it('should return yellow when rating is between 5 and 7', () => {
      expect(component.ratingClass(5)).toBe('yellow');
      expect(component.ratingClass(7)).toBe('yellow');
    });

    it('should return red when rating is below 5', () => {
      expect(component.ratingClass(4)).toBe('red');
      expect(component.ratingClass(1)).toBe('red');
    });
  });

  describe('editRating interaction', () => {
    const activeMovie = { id: 42, title: 'Matrix', rating: 7 };

    it('should exit editRating early if prompt input text values are blank', () => {
      const promptSpy = vi.spyOn(window, 'prompt').mockReturnValue(null);

      component.editRating(activeMovie);

      httpMock.expectNone(`${component.apiUrl}/42/rating`);
      expect(promptSpy).toHaveBeenCalled();
    });

    it('should successfully dispatch a PUT request on user submission entry', () => {
      const promptSpy = vi.spyOn(window, 'prompt').mockReturnValue('9');

      component.editRating(activeMovie);

      const reqPut = httpMock.expectOne(`${component.apiUrl}/42/rating`);
      expect(reqPut.request.method).toBe('PUT');
      expect(reqPut.request.body).toEqual({ rating: 9 });
      reqPut.flush({});

      const reqMovies = httpMock.expectOne(component.apiUrl);
      reqMovies.flush([]);
      const reqAvg = httpMock.expectOne(`${component.apiUrl}/average`);
      reqAvg.flush(0);

      expect(promptSpy).toHaveBeenCalled();
    });
  });
});
