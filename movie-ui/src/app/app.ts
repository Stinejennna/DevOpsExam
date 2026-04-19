import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {

  apiUrl = 'http://localhost:5265/api/movies';

  title = '';
  rating = 1;

  movies: any[] = [];
  average = 0;

  searchText = '';
  sortOption = 'newest';

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.refreshData();
  }

  refreshData() {
    this.loadMovies();
    this.loadAverage();
  }

  loadMovies() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => {
      this.movies = data;
      this.cdr.detectChanges();
    });
  }

  loadAverage() {
    this.http.get<number>(`${this.apiUrl}/average`).subscribe(data => {
      this.average = data;
      this.cdr.detectChanges();
    });
  }

  addMovie() {
    const movie = {
      title: this.title,
      rating: Number(this.rating)
    };

    this.http.post(this.apiUrl, movie).subscribe(() => {
      this.title = '';
      this.rating = 1;
      this.refreshData();
    });
  }

  deleteMovie(id: number) {
    if (!confirm('Delete this movie?')) return;

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.movies = this.movies.filter(movie => movie.id !== id);
        this.loadAverage();
        this.cdr.detectChanges();
      }
    });
  }

  filteredMovies() {
    let filtered = this.movies.filter((movie: any) =>
      movie.title.toLowerCase().includes(this.searchText.toLowerCase())
    );

    switch (this.sortOption) {
      case 'ratingHigh':
        return filtered.sort((a, b) => b.rating - a.rating);

      case 'ratingLow':
        return filtered.sort((a, b) => a.rating - b.rating);

      case 'az':
        return filtered.sort((a, b) => a.title.localeCompare(b.title));

      case 'za':
        return filtered.sort((a, b) => b.title.localeCompare(a.title));

      case 'oldest':
        return filtered.sort((a, b) => a.id - b.id);

      default:
        return filtered.sort((a, b) => b.id - a.id);
    }
  }

  ratingClass(rating: number) {
    if (rating >= 8) return 'green';
    if (rating >= 5) return 'yellow';
    return 'red';
  }
}
