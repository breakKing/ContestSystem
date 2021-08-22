<template>
  <div class="flex-shrink-0 bg-primary d-flex flex-column justify-content-start"
       style="height: 100%; padding: 10px">

    <h4 class="fw-bold m-2 mb-5 text-light">Панель модератора</h4>

    <ul class="list-unstyled ps-0 ">
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#courses-collapse" aria-expanded="false">
          Курсы
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedCoursesPage','ModeratorApprovedCoursesPage','ModeratorRejectedCoursesPage'].includes($route.name)}"
             id="courses-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedCoursesPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedCoursesPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedCoursesPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#posts-collapse" aria-expanded="false">
          Посты
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedPostsPage', 'ModeratorApprovedPostsPage','ModeratorRejectedPostsPage'].includes($route.name)}"
             id="posts-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedPostsPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedPostsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedPostsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#contests-collapse" aria-expanded="false">
          Соревнования
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedContestsPage','ModeratorApprovedContestsPage','ModeratorRejectedContestsPage'].includes($route.name)}"
             id="contests-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedContestsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedContestsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedContestsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#tasks-collapse" aria-expanded="false">
          Задачи
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedProblemsPage','ModeratorApprovedProblemsPage','ModeratorRejectedProblemsPage'].includes($route.name)}"
             id="tasks-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedProblemsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedProblemsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedProblemsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#checkers-collapse" aria-expanded="false">
          Чекеры
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedCheckersPage','ModeratorApprovedCheckersPage','ModeratorRejectedCheckersPage'].includes($route.name)}"
             id="checkers-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedCheckersPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedCheckersPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedCheckersPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "ModerSideBarComponent"
}
</script>

<style lang="scss" scoped>
h4 {
  font-family: cursive;
}

.btn {
  display: inline-flex;
  align-items: center;
  padding: .25rem .5rem;
  font-weight: 600;
  font-size: 22px;
  color: rgba(0, 0, 0, .65);
  background-color: transparent;
  border: 0;
}

.btn:hover,
.btn:focus,
li:hover,
li:focus {
  color: rgba(0, 0, 0, .85);
  background-color: rgba(210, 244, 234, 0.188);
}

.btn-toggle::before {
  width: 1.25em;
  line-height: 0;
  content: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='rgba%280,0,0,.5%29' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M5 14l6-6-6-6'/%3e%3c/svg%3e");
  transition: transform .35s ease;
  transform-origin: .5em 50%;
}

.btn-toggle[aria-expanded="true"] {
  color: rgba(0, 0, 0, .85);
}

.btn-toggle[aria-expanded="true"]::before {
  transform: rotate(90deg);
}

.btn-toggle-nav a {
  display: inline-flex;
  padding: .1875rem .5rem;
  margin-top: .125rem;
  margin-left: 1.25rem;
  text-decoration: none;
}

.router-link-active {
  background-color: #0A5BF0;
}

.link-light {
  font-size: 18px;
}
</style>