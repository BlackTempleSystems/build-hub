import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgottenPasswordPage } from './forgotten-password.page';

describe('ForgottenPasswordPage', () => {
  let component: ForgottenPasswordPage;
  let fixture: ComponentFixture<ForgottenPasswordPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForgottenPasswordPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ForgottenPasswordPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
